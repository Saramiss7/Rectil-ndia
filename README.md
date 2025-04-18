🚀 DESCRIPTION (ENGLISH)

# 📍 Rectilàndia – Route Calculator

This programme calculates the shortest route between two cities in the fictional region of Rectilàndia, where all roads are straight. It connects to a MySQL database provided through a Docker container, containing information about cities and roads.

## 🔧 What does this programme do?

- Connects to a MySQL database with the cities and roads data.
- Asks the user for a starting city and a destination.
- Calculates the shortest route between them based on distance.
- Displays the full route with city names and total distance in kilometres.
- Offers the option to save the route in a text file to keep a history of routes.

## 🗂️ Code structure

The code is divided into different classes and methods to keep it clean, structured, and easy to understand:

- **`Program` class**: Contains the main flow of the programme and method calls.
- **`Process` class**: Manages data needed to calculate the route: pending cities, checked cities, distances, and previous steps.
- **`Rutes` class**: Stores the final result of the calculated route, including the path and total distance.
- **Helper methods**: Handle the database connection, data reading, route reconstruction, and saving results to a file.

## 💾 About the route file

Although it is not included in this repository, the programme saves the routes found in a `.txt` file. Each time a new route is calculated, the user can choose to save it. The file keeps a history of all saved routes without overwriting the previous ones.

## ⚙️ Requirements

- Docker installed to run the database server:

  ```bash
  docker run -p 3306:3306 --rm -d --name rectilandia utrescu/mysql-rectilandia:latest

🚀 DESCRIPCIÓN (SPANISH)
# 📍 Rectilàndia – Calculadora de rutes

Este programa permite calcular la ruta más corta entre dos ciudades de la comarca ficticia de Rectilàndia, donde todas las carreteras son rectas. Utiliza una base de datos proporcionada a través de un contenedor Docker con las ciudades y carreteras disponibles.

## 🔧 ¿Qué hace este programa?

- Se conecta a una base de datos MySQL que contiene la información de las ciudades y las carreteras.
- Pide al usuario una ciudad de origen y una de destino.
- Calcula la ruta más corta entre ambas ciudades, teniendo en cuenta las distancias.
- Muestra por pantalla el recorrido con los nombres de las ciudades y los kilómetros totales.
- Da la opción de guardar la ruta encontrada en un archivo de texto para mantener un historial de rutas.

## 🗂️ Organización del código

El código está dividido en diferentes clases y métodos para mantenerlo limpio, estructurado y fácil de entender:

- **Clase `Program`**: Contiene el flujo principal del programa y las llamadas a los métodos auxiliares.
- **Clase `Process`**: Gestiona los datos necesarios para calcular la ruta: ciudades pendientes, ya comprobadas, distancias y caminos recorridos.
- **Clase `Rutes`**: Guarda el resultado final de la ruta calculada, incluyendo el recorrido y los kilómetros totales.
- **Métodos auxiliares**: Se encargan de la conexión con la base de datos, la lectura de los datos, la reconstrucción del camino, y la opción de guardar el resultado en un archivo.

## 💾 Sobre el archivo de rutas

Aunque no está incluido en este repositorio, el programa guarda las rutas que se encuentran en un archivo `.txt`. Cada vez que el usuario encuentra una nueva ruta, puede optar por guardarla. El archivo mantiene todas las rutas guardadas sin sobrescribir las anteriores.

## ⚙️ Requisitos

- Tener Docker instalado para ejecutar el servidor de base de datos:
  
  ```bash
  docker run -p 3306:3306 --rm -d --name rectilandia utrescu/mysql-rectilandia:latest
