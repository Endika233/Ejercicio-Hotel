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
                case 3:
                    CheckIn();
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
        public static void CheckIn()
        {
            string dni;
            int habSel;
            conexion.Open();

            SqlDataReader match;
           
            Console.WriteLine("\nIntroduzca el DNI del cliente(sin guion)");
            dni = Console.ReadLine();
            cadena = "SELECT * FROM HUESPED WHERE DNI='" + dni + "'";
            comando = new SqlCommand(cadena, conexion);
            match = comando.ExecuteReader();
            if (!match.Read())
            {
                 Console.WriteLine("\nEl cliente no esta registrado");
            }
            else
            {
                match.Close();
                Console.WriteLine("\nSeleccione número de habitación para el huésped\n");
                cadena = "SELECT [NumHab] FROM HABITACION WHERE ESTADO like 'l'";
                comando = new SqlCommand(cadena, conexion);
                SqlDataReader habitL = comando.ExecuteReader();
                while (habitL.Read())
                {
                    Console.Write(habitL["NumHab"]+"  ");
                }
                habitL.Close();
                Console.WriteLine("\n");
                habSel = Int32.Parse(Console.ReadLine());
                cadena = "UPDATE HABITACION SET ESTADO='o' WHERE NumHab = " + habSel ;
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
            }
            match.Close();

            
        }

    }
}
//Check-in: Aquí pediremos el DNI del cliente que quiere hacer la reserva.Si el cliente no existe en la tabla clientes
//aparecerá un mensaje que nos indique que el cliente no está registrado y por lo tanto no puede hacer una reserva. 
//Si el cliente está registrado, le aparecerá un listado con las habitaciones disponibles del hotel para que seleccione 
//la que quiera reservar.Una vez validado que el número de la habitación que ha introducido es correcto,
//tendremos que hacer un update a la tabla de HABITACIONES para poner la habitación como ocupada.
