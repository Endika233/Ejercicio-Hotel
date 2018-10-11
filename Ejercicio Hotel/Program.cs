using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace Ejercicio_Hotel
{
    class Program { 
            static String connectionString = ConfigurationManager.ConnectionStrings["conexionHotel"].ConnectionString;
            static SqlConnection conexion = new SqlConnection(connectionString);
            static string cadena;
            static SqlCommand comando;
    
        static void Main(string[] args)
        {
            Menu();
            Console.ReadLine();
        }
        public static void Menu()
        {
            int option,cont=0;
            Console.WriteLine("\t\t\t\t\tBienvenido a Hotel Boutique Helene");
            do
            {
                if (cont > 0)
                {
                    Console.WriteLine("\nEl valor introducido no es válido, por favor introduzca otro");
                }
                Console.WriteLine("\nQue desea hacer (elija una de estas opciones):\n\n1 Registrar Cliente\n2 Editar Cliente\n3 Check-In\n4 Check-Out\n5 Salir");
                option = Int32.Parse(Console.ReadLine());
                cont++;
            } while (option>5||option<0);
            switch (option)
            {
                case 1:
                    RegistrarCliente();
                    break;
                case 2:
                    EditarCliente();
                    break;
            }
        }
        public static void RegistrarCliente()
        {
            conexion.Open();
            string nomb, ape, dni;
            int cont=0;
            Console.WriteLine("\nIntroduzca nombre del cliente");
            nomb = Console.ReadLine();
            Console.WriteLine("\nIntroduzca el apellido del cliente");
            ape = Console.ReadLine();
            do
            {
                if (cont > 0)
                {
                    Console.WriteLine("\nEl DNI introducido no es correcto (no usar guión)");
                }
                Console.WriteLine("\nIntroduzca DNI del cliente (9 dígitos)");
                dni = Console.ReadLine();
                cont = cont + 1;
            } while (dni.Length!=9);
            cadena = "INSERT INTO HUESPED VALUES ('"+nomb+"', '"+ape+"', '"+dni+"')";
            comando = new SqlCommand(cadena, conexion);
            comando.ExecuteNonQuery();
      

            conexion.Close();
            Menu();
        }
        public static void EditarCliente()
        { string dni,nomb,ape;
            conexion.Open();            
            int cont = 0;
            bool ok = false;
            SqlDataReader match;
            do
            {
                if (cont > 0)
                {
                    Console.WriteLine("\nEl DNI introducido no es correcto (no usar guión)");
                }
                Console.WriteLine("\nIntroduzca el DNI del cliente que desee editar");
                dni = Console.ReadLine();
                cadena = "SELECT * FROM HUESPED WHERE DNI='" + dni + "'";
                comando = new SqlCommand(cadena, conexion);
                match = comando.ExecuteReader();     
                if (match.Read())
                {
                    ok = true;
                }
                cont++;
                match.Close();
            } while (!ok);
            Console.WriteLine("\nIntroduzca el nuevo nombre ");
            nomb = Console.ReadLine();
            Console.WriteLine("\nIntroduzca el nuevo apellido");
            ape = Console.ReadLine();
            cadena = "UPDATE HUESPED SET nombre='" + nomb + "',apellido='" + ape + "' WHERE DNI LIKE '"+dni+"'";
            comando = new SqlCommand(cadena, conexion);
            comando.ExecuteNonQuery();

            conexion.Close();
        }

    }
}
//Registrar cliente: Aquí registramos un nuevo cliente, puesto que no se puede reserva una habitación si previamente
//no se a registrado al cliente.Haremos un método RegistrarCliente() en el cual pediremos por teclado
//el nombre, apellido y DNI y haremos un Insert a la tabla de Clientes.

//Editar cliente: Aquí tendremos la opción de cambiar el nombre y el apellido de un cliente que ya está registrado en la BBDD.
//para ello crearemos un método EditarCliente(String DNI). Pediremos por teclado el DNI del cliente al que queremos cambiar 
//los datos.Nos lo pedirá continuamente  hasta que el DNI introducido sea correcto.
//Entonces le pediremos que nos introduzca el Nombre y el Apellido de nuevo.

//Check-in: Aquí pediremos el DNI del cliente que quiere hacer la reserva.Si el cliente no existe en la tabla clientes
//aparecerá un mensaje que nos indique que el cliente no está registrado y por lo tanto no puede hacer una reserva. 

//Si el cliente está registrado, le aparecerá un listado con las habitaciones disponibles del hotel para que seleccione 
//la que quiera reservar.Una vez validado que el número de la habitación que ha introducido es correcto,
//tendremos que hacer un update a la tabla de HABITACIONES para poner la habitación como ocupada.
