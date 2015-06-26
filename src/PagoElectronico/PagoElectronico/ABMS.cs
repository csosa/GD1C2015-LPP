using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;

namespace PagoElectronico
{
    class ABMS
    {
        Conexion con1 = new Conexion();
        SqlDataReader dr;


        //ABM CLIENTE
        public string insertarCliente(string Nombre, string Apellido, string Tipo_ID, int Numero_ID, string Mail, DateTime Nacimiento, string Nacionalidad,int id_domicilio, string usuario)
        {
            con1.cnn.Open();
           
            string salida = "Se registro el Cliente correctamente";

            try
            {
                string query = "INSERT INTO LPP.CLIENTES" +
                                " (num_doc, apellido, nombre, fecha_nac, mail, id_tipo_Doc, id_pais, id_domicilio,username) " +
                                "VALUES (" + Numero_ID + ", '" + Apellido + "', '" + Nombre + "', '" + Nacimiento+ "',"
                                +" '" + Mail + "', (select tipo_cod FROM LPP.TIPO_DOCS WHERE tipo_descr = '" + Tipo_ID + "')" +
                                ", (select id_pais from LPP.PAISES WHERE pais like '%" + Nacionalidad + "' )," + id_domicilio + ",'"+usuario+"') ";
                
                SqlCommand command = new SqlCommand(query, con1.cnn);
                command.ExecuteNonQuery();

            }
            
            catch(Exception ex)
            {
              salida = "No se pudo insertar el Cliente"+ ex.ToString() ;
            }


            con1.cnn.Close();  
            return salida;
             
        }
    
        public string modificarCliente(string Nombre, string Apellido, string Tipo_ID, int Numero_ID, string Mail, DateTime Nacimiento,string Nacionalidad,string Username)
        {
            con1.cnn.Open();
            string salida = "Se modificó el Cliente correctamente";

            try
            {
                string query = "UPDATE LPP.CLIENTES SET fecha_nac = CONVERT(DATETIME, '" + Nacimiento+ "', 103)"
                    +", mail = '" + Mail 
                    + "', apellido = '" + Apellido 
                    + "', nombre = '" + Nombre 
                    + "',  id_pais=(select id_pais from LPP.PAISES where pais like '%" + Nacionalidad +"')"
                    +", id_tipo_doc = ( select tipo_cod from LPP.TIPO_DOCS where tipo_descr='"+Tipo_ID +"'"
                    + "), num_doc = "+ Numero_ID
                    +" WHERE username = '"+Username +"'";
                SqlCommand command = new SqlCommand(query, con1.cnn);
                command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                salida = "No se pudo modificar el Cliente" + ex.ToString();
            }

            con1.cnn.Close();  
            return salida;

        }

        public string eliminarCliente(string Tipo_ID, int Numero_ID)
        {
            con1.cnn.Open();
            string salida = "Se deshabilito el Cliente correctamente";

            try
            {
                string query = "UPDATE LPP.CLIENTES SET habilitado = 0 WHERE id_tipo_doc = (select tipo_cod from LPP.TIPO_DOCS where tipo_descr = '" +Tipo_ID + "') AND num_doc = '" + Numero_ID + "'";
                SqlCommand command = new SqlCommand(query, con1.cnn);
                command.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                salida = "No se pudo deshabilitar el cliente" + ex.ToString();
            }

            con1.cnn.Close();  
            return salida;

        }

        //ABM DOMICILIO
        public int insertarDomicilio(string calle, int numero, int Piso, string depto, string localidad)
        {
            string salida = "Se registro DOMICILIO correctamente";
            con1.cnn.Open();
            try
            {
                string query = "INSERT INTO LPP.DOMICILIOS (calle, num, piso, depto, localidad)" +
                               " VALUES ('" + calle + "'," + numero + "," + Piso + ",'"+ depto +"', '"+ localidad +"')";
                              
                
                SqlCommand command = new SqlCommand(query, con1.cnn);
                command.ExecuteNonQuery();
                

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                salida = "No se pudo insertar" + ex.ToString();
            }
            con1.cnn.Close();


            Conexion con3 = new Conexion();
            string query3 = "SELECT id_domicilio FROM LPP.DOMICILIOS WHERE calle = '"+calle+"' AND num = "+numero+"AND piso= "+Piso+" ";
            con3.cnn.Open();
            SqlCommand command3 = new SqlCommand(query3, con3.cnn);
            SqlDataReader lector3 = command3.ExecuteReader();
            lector3.Read();
            int id_domicilio = lector3.GetInt32(lector3.GetOrdinal("id_domicilio"));
            con3.cnn.Close();

            return id_domicilio;

        }
      

        public void modificarDomicilio(string calle, int numero, int Piso, string depto, string localidad, int id_domicilio)
        {
            string salida = "Se modificó el DOMICILIO correctamente";
            con1.cnn.Open();
            try
            {
                string query = "UPDATE LPP.DOMICILIOS SET calle = '" + calle +"',  num = " + numero + ","+
                               " piso = " +Piso + ", depto = '"+ depto+"', localidad = '" +localidad+"'" +
                               " WHERE id_domicilio = "+id_domicilio+"";
                
                SqlCommand command = new SqlCommand(query, con1.cnn);
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                salida = "No se pudo modificar" + ex.ToString();
            }

            con1.cnn.Close();
            
        }

        

        




    }
}
