﻿using System;
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
            Console.WriteLine("\t\t\t\t\tBienvenido a Hotel Boutique Helene");
            Menu();
            Console.ReadLine();
        }
        public static void Menu()
        {
            int option, cont = 0;                     
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
                case 4:
                    CheckOut();
                    break;
                case 5:
                    Salir();
                    break; 
            }
        }
        public static void RegistrarCliente()
        {
            Console.WriteLine("\n\tHa elegido la opción Registrar Cliente");
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
                    Console.WriteLine("\nEl DNI introducido no es correcto (no usar guión) o introduzca 'esc' para salir del menú Registrar Cliente");
                }
                Console.WriteLine("\nIntroduzca DNI del cliente (9 dígitos)");
                dni = Console.ReadLine().ToUpper();
                cont = cont + 1;
            } while (dni.Length!=9&&dni!="ESC");
            conexion.Open();
            if (dni.Length == 9)
            {
                cadena = "INSERT INTO HUESPED VALUES ('" + nomb + "', '" + ape + "', '" + dni + "')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
            }      
            conexion.Close();
            Menu();
        }
        public static void EditarCliente()
        {
            Console.WriteLine("\n\tHa elegido la opción Editar Cliente");
            string dni,nomb,ape;
            int cont = 0;
            bool ok = false;
            SqlDataReader match;
            conexion.Open();            
            do
            {
                if (cont > 0)
                {
                    Console.WriteLine("\nEl DNI introducido no es correcto (no usar guión)");
                }
                Console.WriteLine("\nIntroduzca el DNI del cliente que desee editar o introduzca 'esc' para salir del Editor de Clientes");
                dni = Console.ReadLine().ToUpper();
                cadena = "SELECT * FROM HUESPED WHERE DNI LIKE '" + dni + "'";
                comando = new SqlCommand(cadena, conexion);
                match = comando.ExecuteReader();     
                if (match.Read())
                {
                    ok = true;
                }
                cont++;
                match.Close();
            } while (!ok&&dni!="ESC");
            if (ok)
            {
                Console.WriteLine("\nIntroduzca el nuevo nombre ");
                nomb = Console.ReadLine();
                Console.WriteLine("\nIntroduzca el nuevo apellido");
                ape = Console.ReadLine();
                cadena = "UPDATE HUESPED SET nombre='" + nomb + "',apellido='" + ape + "' WHERE DNI LIKE '" + dni + "'";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
            }
            conexion.Close();
            Menu();
        }
        public static void CheckIn()
        {
            Console.WriteLine("\n\tHa elegido la opción CheckIn");
            string dni;
            int habSel, newCodReserva=0;//TODO: Podias haber usado el AUTO_INCREMENT para el nuevo codreserva??(identity(1,1) en sql server)**Primero es el número, segundo la suma
            SqlDataReader match,codReserva;
           
            conexion.Open();
            Console.WriteLine("\nIntroduzca el DNI del cliente(sin guion)");
            dni = Console.ReadLine();
            cadena = "SELECT * FROM HUESPED WHERE DNI LIKE '" + dni + "'";
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
                    Console.Write(habitL["NumHab"]+"  ");//LA LISTA DE LAS HABITACIONES LIBRES
                }
                Console.WriteLine();
                habitL.Close();
                habSel = Int32.Parse(Console.ReadLine());
                cadena = "UPDATE HABITACION SET ESTADO='o' WHERE NumHab = " + habSel ;
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                
                cadena = "SELECT MAX(CODRESERVA) as 'CodReserva' FROM RESERVAS ";//Si no le damos un nombre aqui a la busqueda, dentro del if no funciona con el nombre porque no le asignas un nombre 'as'
                comando = new SqlCommand(cadena, conexion);
                codReserva=comando.ExecuteReader();
                if (codReserva.Read())//if aquí porque el programa no sabe si cogerá algún valor o no
                {
                    newCodReserva = Int32.Parse(codReserva["CodReserva"].ToString())+1;
                }
                codReserva.Close();

                cadena = "INSERT INTO RESERVAS (CODRESERVA,DNI_HUESPED,NUMHAB,CHECKIN) VALUES (" + newCodReserva+",'"+dni+"',"+habSel+",'"+ DateTime.UtcNow.ToString() +"')";//TODO:1Hora universal metida, las otras daban error 2mirar como sumar dos horas ELSE cambiar datetime en BBDD a date
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
            }
            match.Close();
            conexion.Close();
            Menu();
        }
        public static void CheckOut()
        {
            Console.WriteLine("\n\tHa elegido la opción CheckOut");
            string dni;
            SqlDataReader match;

            conexion.Open();
            Console.WriteLine("\nIntroduzca el DNI del cliente(sin guion)");
            dni = Console.ReadLine();
            cadena = "SELECT * FROM RESERVAS WHERE CHECKOUT IS NULL AND DNI_HUESPED LIKE '" + dni + "'";
            comando = new SqlCommand(cadena, conexion);
            match = comando.ExecuteReader();
            if (!match.Read())
            {
                Console.WriteLine("\nEl cliente no se encuentra en el hotel o no esta registrado");
            }
            else
            {
                match.Close();
                cadena = "UPDATE RESERVAS SET CHECKOUT='" + DateTime.UtcNow.ToString() + "' WHERE DNI_HUESPED LIKE '"+dni+"' AND CODRESERVA=(SELECT MAX(CODRESERVA) FROM RESERVAS WHERE DNI_HUESPED LIKE '"+dni+"')";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                cadena = "UPDATE HABITACION SET ESTADO='l' WHERE NUMHAB=(SELECT NUMHAB FROM RESERVAS WHERE CODRESERVA=(SELECT MAX(CODRESERVA) FROM RESERVAS WHERE DNI_HUESPED LIKE '"+dni+"'))";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
            }
            match.Close();
            conexion.Close();
            Menu();
        }
        public static void Salir()
        {
            Console.WriteLine("\nQue tenga un buen día");
        }
        //public static void VerHabs()
        //{
        //    int option,cont=0;
        //    conexion.Open();
        //    Console.WriteLine("\n\tHa seleccionado el menú Ver Habitaciones");
        //    do
        //    {
        //        if (cont > 0)
        //        {
        //            Console.WriteLine("\nEl valor introducido no es válido, por favor introduzca otro");
        //        }
        //        Console.WriteLine("\nQue desea hacer (elija una de estas opciones):\n\n1 Ver todas las habitaciones\n2 Ver habitaciones ocupadas\n3Ver habitaciones vacias\n4 Salir");
        //        option = Int32.Parse(Console.ReadLine());
        //        cont++;
        //    } while (option > 4 || option < 0);
        //    switch (option)
        //    {
        //        case 1:
                    
        //            break;
        //        case 2:
                   
        //            break;
        //        case 3:
                   
        //            break;
        //        case 4:                    
        //            break;
        //    }
        //    conexion.Close();
        //    }        
    }
}

