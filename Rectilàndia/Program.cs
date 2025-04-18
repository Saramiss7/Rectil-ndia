using System.Data;
using MySql.Data.MySqlClient;
namespace Rectilàndia;

class Program
{
    //docker run -p 3306:3306 --rm -d --name rectilandia utrescu/mysql-rectilandia:latest
    public static Rutes ruta = new();
    public static RutaData p = new();
    public static long Ciutatactual { get; set; }

    static void Main()
    {
        var con = DBConnexion();
        if (con == null || con.State != ConnectionState.Open)
        {
            Console.WriteLine("Unable to establish connection to the database");
            return;
        }
        var (origen, desti, idDesti) = EntradaUsuari(con);
        if (idDesti == -1) return;
        
       var ciutats = Dataset("SELECT * FROM Ciutats", "Ciutats", con);
       var carreteres = Dataset("SELECT * FROM Carreteres", "Carreteres", con);

       while (p.Pendents.Count > 0)
       {
           CalcularRuta(carreteres, idDesti);
       }
       
       if (RutaExist(idDesti))
       {
           ReconstruirCami(idDesti);
           var ruta = FinalText(origen, desti, ciutats);
           AskSaving(ruta);
       }
       else
       {
           Console.WriteLine("No route to the destination could be found");
       }
       
       con.Close();
    }
    
    static MySqlConnection? DBConnexion()
    {
        try
        {
            var con = new MySqlConnection("Server=127.0.0.1;port=3306;user id=root;password=recte;database=rectilandia;");
            con.Open();
            return con;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Database connection error: " + ex.Message);
            return null;
        }
    }
    static (string, string,long) EntradaUsuari(MySqlConnection con)
    {
        string origen = "";
        string desti = "";
        long idDesti = -1;
        try
        {
            //Origen
            Console.WriteLine("Origen");
            origen = Console.ReadLine();
            if (string.IsNullOrEmpty(origen))
            {
                Console.WriteLine("Invalid Entry");
                return ("","",-1);
            }

            long IdOrigen = IdCiutat(origen, con);
            if (IdOrigen == -1)
            {
                Console.WriteLine("City not found");
                return ("","",-1);
            }

            Ciutatactual = IdOrigen;
            p.Pendents.Add(Ciutatactual);
            p.Distancies[Ciutatactual] = 0;

            //Destí
            Console.WriteLine("Destí");
            desti = Console.ReadLine();
            if (string.IsNullOrEmpty(desti))
            {
                Console.WriteLine("Invalid Entry");
                return ("","",-1);
            }

            idDesti = IdCiutat(desti, con);
            if (idDesti == -1){
                Console.WriteLine("City not found");
                return ("","",-1);
            }
            return (origen, desti, idDesti);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR" + ex.Message);
            return ("","",-1);
        }
    }
    static long IdCiutat(string nomCity, MySqlConnection con) {
        MySqlCommand cmd = new($"SELECT Id FROM Ciutats where Nom = @nom", con);
        cmd.Parameters.AddWithValue("@nom", nomCity);
        try
        {
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return reader.GetInt32("Id");
            else
            {
                Console.WriteLine($"'{nomCity}' does not exist");
                return -1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return -1;
        }
    }
    static DataSet Dataset(string consulta, string nomTaula, MySqlConnection con)
    {
        DataSet dataset = new();
        try
        {
            dataset.Tables.Add(nomTaula);
            MySqlDataAdapter adapter = new(consulta, con);
            adapter.Fill(dataset, nomTaula);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR {nomTaula}: {ex.Message}");
        }
        return dataset;
    }

    private static void CalcularRuta(DataSet carreteres, long idDesti)
    {
        //De les ciutats pendents, comprovem la que té menor distància des de la ciutatactual
        long ciutatActual = -1;
        int minKm = int.MaxValue;
        foreach (var idpendent in p.Pendents) {
            if (p.Distancies[idpendent] < minKm)
            {
                minKm = p.Distancies[idpendent];
                ciutatActual = idpendent;
            }
        }
        if (ciutatActual == -1) return;

        //Un cop comprovada la pasem a la llista de Comprovats
        p.Pendents.Remove(ciutatActual);
        p.Comprovats.Add(ciutatActual);
        
        //En cas de haber arribat al destí, sortim del bucle
        if (ciutatActual == idDesti) return;

        //Trobem més ciutats des de la ciutat en la qual ens trobem actualment
        DataRow[] resultat = carreteres.Tables["Carreteres"].Select($"Origen = '{ciutatActual}'");
        if (!carreteres.Tables.Contains("Carreteres"))
        {
            Console.WriteLine("ERROR: Table 'Carreteres' not found.");
            return;
        }

        foreach (DataRow fila in resultat)
        {
            //Convertim les variables a integer per a quà no pensi que són objectes
            long idDestiProxim = Convert.ToInt32(fila["Desti"]);
            int kms = Convert.ToInt32(fila["Kilometres"]);

            //Si ja hem comprovat la ciutat ens la saltem
            if (p.Comprovats.Contains(idDestiProxim)) continue; // torna a l'inici del bucle

            //En cas d'escollir la ciutat com continuació de la ruta, calcular els kilometres totals de moment
            int novaDist = p.Distancies[ciutatActual] + kms;

            //Si no hem passat anteriorment o si la distància és menor
            if (!p.Distancies.ContainsKey(idDestiProxim) || novaDist < p.Distancies[idDestiProxim])
            {
                p.Distancies[idDestiProxim] = novaDist;
                p.Anterior[idDestiProxim] = ciutatActual;
                if (!p.Pendents.Contains(idDestiProxim)) {
                    p.Pendents.Add(idDestiProxim);
                }
            }
        }
        if (p.Pendents.Count == 0 && ciutatActual == -1)
        {
            return;
        }
    }
    
    static bool RutaExist(long idDesti)
    {
        return p.Distancies.ContainsKey(idDesti);
    }
    
    static void ReconstruirCami(long destifinal) {
        List<long> camiFinal = new();
        long desti = destifinal;
        //Anem tornant enrere mentre anem emmagatzemant per ordre les ciutats per les quals hem passat
        while (desti != Ciutatactual)
        {
            camiFinal.Insert(0, desti);// afegim la ciutat al recorregut
            if (!p.Anterior.ContainsKey(desti))
            {
                camiFinal.Clear();
                break;
            }
            desti = p.Anterior[desti]; //Abancem cap enrere
        }
        camiFinal.Insert(0, Ciutatactual);
        //Guardem
        ruta.Recorregut = camiFinal;
        if (p.Distancies.ContainsKey(destifinal))
        {
            ruta.km = p.Distancies[destifinal];
        }
        else {
            Console.WriteLine("No route found to destination.");
        }
    }
    static string FinalText(string origen, string desti, DataSet ciutats)
    {
        string ruta = "";
        ruta += $"Shortest route from {origen} to {desti}\n";
        foreach (var ciutat in Program.ruta.Recorregut)
        {
            var resultat = ciutats.Tables["ciutats"].Select($"Id = {ciutat}");
            if (resultat.Length > 0)
            {
                var nom = Convert.ToString(resultat[0]["Nom"]);
                ruta += $"-> {nom}";
            }
        }
        ruta += $"\nTotal kilometers: {Program.ruta.km}\n\n";
        Console.WriteLine(ruta);
        return ruta;
    }
    
    private static void AskSaving(string ruta)
    {
        Console.WriteLine("\nDo you want to save the route? y/n");
        var answer = Console.ReadLine();
        if (answer == "y")
        {
            if (Program.ruta.Recorregut.Count > 0 && Program.ruta.km >= 0)
            {
                File.AppendAllText("RoutesFound.txt", ruta);
                Console.WriteLine("Route saved!");
            }
            else Console.WriteLine("No route to save.");
        }
    }

}