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
using Helper;
using readConfiguracion;

namespace PagoElectronico
{
    public partial class ABMCliente : Form
    {

        public MenuPrincipal padre_mp;
       
        public ABM_Cliente.BuscarCliente padre_buscar;
        public string evento;
        public string tipoDoc = "";
        public decimal nroDoc = 0;
        public int id = 0;
        private DateTime fechaNac;
        int ban;
        string user;
        ABMS abm = new ABMS();
        public DateTime fecha = DateTime.ParseExact(readConfiguracion.Configuracion.fechaSystem(), "yyyy-dd-MM", System.Globalization.CultureInfo.InvariantCulture);
        
        ErrorProvider errorProvider1 = new ErrorProvider();
        public bool crear;
        

        public ABMCliente(string ev,string usuario)
        {
            InitializeComponent();
            evento = ev;
            user = usuario;
            boxDatosCliente.Enabled = false;
            txtNombre.Enabled = false;
            btnModificar.Enabled = false;
            btnGrabar.Enabled = false;
            btnEliminar.Enabled = false;
            btnLimpiar.Enabled = false;
            btnSalir.Enabled = true;
            btnBuscar.Enabled = true;
            btnNuevo.Enabled = true;
            txtUsuario.Enabled = false;
            fechaNacimiento.Value = fecha;
            lblMailExistente.Visible = false;
            txtUsuario.Text = usuario;
            // Conectar a DB
            Conexion con1 = new Conexion();
            Conexion conDomicilio = new Conexion();
            con1.cnn.Open();

            // Pregunto todos los TipoDoc de la DB
            
            string query = "SELECT tipo_descr FROM LPP.TIPO_DOCS";
            SqlCommand command = new SqlCommand(query, con1.cnn);
            SqlDataReader lector1 = command.ExecuteReader();
            while (lector1.Read())
            {
                // Cargo la descripciones en la lista
                cbID.Items.Add(lector1.GetString(0));
            }

            con1.cnn.Close();
            // Pregunto todos los TipoDoc de la DB
            con1.cnn.Open();
            string query2 = "SELECT pais FROM LPP.PAISES ORDER BY pais";
            SqlCommand command2 = new SqlCommand(query2, con1.cnn);
            SqlDataReader lector2 = command2.ExecuteReader();
            while (lector2.Read())
            {
                // Cargo la descripciones en la lista
                cb2.Items.Add(lector2.GetString(0));
            }

            con1.cnn.Close();

            if(evento != "A")
            {
                string query1 = "SELECT d.tipo_descr, num_doc, nombre, apellido, " + 
                                " p.pais, fecha_nac,id_domicilio, mail "+
                                " FROM LPP.CLIENTES cl LEFT JOIN LPP.PAISES p ON cl.id_pais=p.id_pais "+
                                " LEFT JOIN LPP.TIPO_DOCS d ON cl.id_tipo_doc = d.tipo_cod WHERE username = '" + evento + "'";
                con1.cnn.Open();
                SqlCommand command1 = new SqlCommand(query1, con1.cnn);
                SqlDataReader lector = command1.ExecuteReader();
                
                lector.Read();
                cbID.Text = lector.GetString(0);
                tipoDoc = lector.GetString(0);
                txtNumeroID.Text = lector.GetDecimal(1).ToString();
                nroDoc = lector.GetDecimal(1);
                txtNombre.Text = lector.GetString(2);
                txtApellido.Text = lector.GetString(3);
                cb2.Text = lector.GetString(4); 
                fechaNacimiento.Value = Convert.ToDateTime(lector.GetDateTime(5));
                int id_domicilio = lector.GetInt32(6);
                txtMail.Text = lector.GetString(7);
                txtMail.Enabled = false;
                txtUsuario.Text = evento;
                txtUsuario.Enabled = false;
                con1.cnn.Close();
                

                //Consulto Domicilio
                string queryDomicilio = "SELECT DISTINCT calle,num,depto,piso,localidad " +
                                        "FROM LPP.DOMICILIOS " +
                                        "WHERE id_domicilio = "+ id_domicilio +" ";

                conDomicilio.cnn.Open();
                SqlCommand commandDomicilio = new SqlCommand(queryDomicilio, conDomicilio.cnn);
                SqlDataReader lectorDomicilio = commandDomicilio.ExecuteReader();

                lectorDomicilio.Read();


                //Cargo Datos Domicilio
                
                if (!lectorDomicilio.IsDBNull(0))
                {
                    txtDomicilio.Text = lectorDomicilio.GetString(0);
                }
                if (!lectorDomicilio.IsDBNull(1))
                {
                    txtNumeroCalle.Text = lectorDomicilio.GetDecimal(1).ToString();
                }
                
                if (!lectorDomicilio.IsDBNull(2))
                {
                    txtDepto.Text = lectorDomicilio.GetString(2);
                }
                if (!lectorDomicilio.IsDBNull(3))
                {
                    txtPiso.Text = lectorDomicilio.GetDecimal(3).ToString();
                }
                
                if (!lectorDomicilio.IsDBNull(4))
                {
                    txtLocalidad.Text = lectorDomicilio.GetString(4);
                }
                
                
                
                btnModificar.Enabled = true;
                btnNuevo.Enabled = false;
                btnEliminar.Enabled = true;
            }
            else
            {
                fechaNacimiento.Value = Convert.ToDateTime("2000-01-01");
            }
            if (user != "U")

            {
                txtUsuario.Text = user;
            
            }
        
        }
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            
            ban = 1;
            chkMail.Enabled = false;
            txtNombre.Enabled = true;
            lblNombre.Enabled = true;
            boxDatosCliente.Enabled = true;
            btnLimpiar.Enabled = true;
            btnGrabar.Enabled = true;
            btnNuevo.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
            btnBuscar.Enabled = false;
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNombre_Validating(object sender, CancelEventArgs e)
        {
            
            if (txtNombre.Text == "")
            {
                errorProvider1.SetError(txtNombre,"No ingreso nombre");
                e.Cancel = true;
                return;
            }
            else
            {
               errorProvider1.Clear();
            }
        }

        private void txtApellido_Validating(object sender, CancelEventArgs e)
        {
            if (txtApellido.Text == "")
            {
                errorProvider1.SetError(txtApellido, "No ingreso Apellido");
                e.Cancel = true;
                return;
            }
            else
            {
               errorProvider1.Clear();
            }
        }

        private void txtMail_Validating(object sender, CancelEventArgs e)
        {
            Regex r = new Regex(@"^(.+\@.+\.[a-z]{1,3}.+)$");
            if (r.IsMatch(txtMail.Text))
            { errorProvider1.Clear(); 
            }
            else
            {
                errorProvider1.SetError(txtMail, "Mail Inválido");
                e.Cancel = true;
            }
        }

        private void txtNacionalidad_Validating(object sender, CancelEventArgs e)
        {
            if (cb2.Text == "")
            {
                errorProvider1.SetError(cb2, "No ingreso Pais");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.Clear();
            }
        }

        private void txtDireccion_Validating(object sender, CancelEventArgs e)
        {
            if (txtDomicilio.Text == "")
            {
                errorProvider1.SetError(txtDomicilio, "No ingreso Direccion");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.Clear();
            }
        }
        private bool fechaMayorAHoy(int fecha)
        {
            bool ret;
            int hoy;
            DateTime fechaInicio = DateTime.Today;
            hoy = Convert.ToInt32(fechaInicio.ToString("yyyy-MM-dd").Replace('-', '0'));

            if (fecha >= hoy)
            {
                ret = true;
            }
            else
            {
                ret = false;
            }

            return ret;
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            Conexion con1 = new Conexion();
            int temp;
            decimal temp2;

            DateTime fecha = DateTime.ParseExact(readConfiguracion.Configuracion.fechaSystem(), "yyyy-dd-MM", System.Globalization.CultureInfo.InvariantCulture);

            if (!((DateTime.Compare(fechaNacimiento.Value, fecha)) < 0))
            {

                MessageBox.Show("La fecha de nacimiento debe ser menor a" + fecha);
                return;
            }
            else
            {
                if (cbID.Text!="")
                {
                    string query = "SELECT 1 FROM LPP.TIPO_DOCS WHERE tipo_descr = '" + cbID.Text + "'";
                    con1.cnn.Open();
                    SqlCommand command = new SqlCommand(query, con1.cnn);
                    SqlDataReader lector = command.ExecuteReader();
                    if (!lector.Read())
                    {
                        MessageBox.Show("Tipo de documento Inválido");
                        con1.cnn.Close();
                        return;
                    }
                   con1.cnn.Close();
                }
               
                  
                    // Con este try y este catch verifico que no tengan letras ciertos campos
                    try
                    {
                         if (txtNumeroID.Text != "")
                            temp = Convert.ToInt32(txtNumeroID.Text);
                         if (txtPiso.Text != "")
                             temp2 = Convert.ToDecimal(txtPiso.Text);
                    }
                    catch (Exception h)
                    {
                        MessageBox.Show("Numero de ID y Piso solo pueden contener numeros",h.ToString());
                        return;
                    }

                   this.grabarEnTabla();
                        
                        
                    

            }
        }

        private void grabarEnTabla()
        {
            // Conectar a DB
            Conexion con1 = new Conexion();
            fechaNac = DateTime.Parse(fechaNacimiento.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show(""+fechaNac);
            // Inserto Cliente
            if (txtNombre.Text != "" && txtApellido.Text != "" && cbID.Text != "" && txtNumeroID.Text != "" && txtMail.Text != "" && cb2.Text != "" && fechaNacimiento.Value != null && txtDomicilio.Text != "" && txtNumeroCalle.Text !="")
            {
                
                if (ban == 1)
                {      
                    if(mismoID()==false)
                    {
                        //Compruebo que no se repitan los mails
                        string query2 = "SELECT 1 FROM LPP.CLIENTES WHERE mail = '" + txtMail.Text + "' ";
                        con1.cnn.Open();
                        SqlCommand command2 = new SqlCommand(query2, con1.cnn);
                        SqlDataReader lector2 = command2.ExecuteReader();


                        if (lector2.Read())
                        {

                            MessageBox.Show("Mail existente en la Base de Datos, modifiquelo");
                            lblMailExistente.Visible = true;
                            return;
                        }

                        con1.cnn.Close();

                        if (txtPiso.Text == "")
                        {
                            txtPiso.Text = "0";
                        }
                        if (txtDepto.Text == "")
                        {
                            txtDepto.Text = "NULO";
                        }

                        //Inserto en la Tabla Domicilio
                        int id_domicilio = abm.insertarDomicilio(txtDomicilio.Text, Convert.ToInt32(txtNumeroCalle.Text), Convert.ToInt32(txtPiso.Text), txtDepto.Text, txtLocalidad.Text);
                        string salida = abm.insertarCliente(txtNombre.Text, txtApellido.Text, cbID.Text, Convert.ToInt32(txtNumeroID.Text), txtMail.Text,fechaNac, cb2.Text, id_domicilio,txtUsuario.Text);
                        MessageBox.Show("" + salida);
                    
                    }
                else
                {
                    MessageBox.Show("Ya existe ese Número y Tipo de Identificacion");
                    return;
                }

                    
             }
                    //Modifico al Cliente
                 if (ban == 2)
                    {
                        //Saco id_domicilio para modificar
                        Conexion con3 = new Conexion();
                        string query3 = "SELECT id_domicilio FROM LPP.CLIENTES WHERE id_tipo_doc = (select tipo_cod from LPP.TIPO_DOCS WHERE tipo_descr = '" + tipoDoc + "') AND num_doc= " + nroDoc + " ";
                        con3.cnn.Open();
                        SqlCommand command3 = new SqlCommand(query3, con3.cnn);
                        Int32 id_domicilio = Convert.ToInt32(command3.ExecuteScalar());
                        con3.cnn.Close();

                        if (chkMail.Checked == true)
                        {
                           //Compruebo que no se repitan los mails
                            Conexion con2 = new Conexion();

                            string query2 = "SELECT 1 FROM LPP.CLIENTES WHERE mail = '" + txtMail.Text + "' ";
                            con2.cnn.Open();
                            SqlCommand command2 = new SqlCommand(query2, con2.cnn);
                            SqlDataReader lector2 = command2.ExecuteReader();

                            if (lector2.Read())
                            {

                                MessageBox.Show("Mail existente en la Base de Datos, modifiquelo");
                                lblMailExistente.Visible = true;
                                return;
                            }

                            con2.cnn.Close();

                            if (tipoDoc == cbID.Text && nroDoc == Convert.ToDecimal(txtNumeroID.Text))
                            { //Modifico en la Tabla Domicilio y CLiente
                                abm.modificarDomicilio(txtDomicilio.Text, Convert.ToInt32(txtNumeroCalle.Text), Convert.ToInt32(txtPiso.Text), txtDepto.Text, txtLocalidad.Text, id_domicilio);
                                string salida = abm.modificarCliente(txtNombre.Text, txtApellido.Text, cbID.Text, Convert.ToInt32(txtNumeroID.Text), txtMail.Text, fechaNac, cb2.Text, txtUsuario.Text);
                                MessageBox.Show("" + salida);
                            }
                            else
                            {
                                if (mismoID())
                                {
                                    MessageBox.Show("Ese Numero y Tipo de Identificacion ya existe");
                                    return;
                                }
                                else
                                {
                                    //Modifico en la Tabla Domicilio y CLiente
                                    abm.modificarDomicilio(txtDomicilio.Text, Convert.ToInt32(txtNumeroCalle.Text), Convert.ToInt32(txtPiso.Text), txtDepto.Text, txtLocalidad.Text, id_domicilio);
                                    string salida = abm.modificarCliente(txtNombre.Text, txtApellido.Text, cbID.Text, Convert.ToInt32(txtNumeroID.Text), txtMail.Text, fechaNac, cb2.Text, txtUsuario.Text);
                                    MessageBox.Show("" + salida);
                                }
                            }

                        }

                        else
                        {
                            if (tipoDoc == cbID.Text && nroDoc == Convert.ToDecimal(txtNumeroID.Text))
                            {   //Modifico en la Tabla Domicilio y Cliente
                                abm.modificarDomicilio(txtDomicilio.Text, Convert.ToInt32(txtNumeroCalle.Text), Convert.ToInt32(txtPiso.Text), txtDepto.Text, txtLocalidad.Text, id_domicilio);
                                string salida = abm.modificarCliente(txtNombre.Text, txtApellido.Text, cbID.Text, Convert.ToInt32(txtNumeroID.Text), txtMail.Text, fechaNac, cb2.Text, txtUsuario.Text);
                                MessageBox.Show("" + salida);
                            }
                            else
                            {
                                if (mismoID())
                                {
                                    MessageBox.Show("Ese Numero y Tipo de Identificacion ya existe");
                                    return;
                                }
                                else
                                {
                                    //Modifico en la Tabla Domicilio y CLiente
                                    abm.modificarDomicilio(txtDomicilio.Text, Convert.ToInt32(txtNumeroCalle.Text), Convert.ToInt32(txtPiso.Text), txtDepto.Text, txtLocalidad.Text, id_domicilio);
                                    string salida = abm.modificarCliente(txtNombre.Text, txtApellido.Text, cbID.Text, Convert.ToInt32(txtNumeroID.Text), txtMail.Text, fechaNac, cb2.Text, txtUsuario.Text);
                                    MessageBox.Show("" + salida);
                                }

                            }

                        }

                      
                    }
                    limpiarDatos();
                    lblNombre.Enabled = false;
                    txtNombre.Enabled = false;
                    btnLimpiar.Enabled = true;
                    btnGrabar.Enabled = false;
                    btnBuscar.Enabled = true;
                    btnNuevo.Enabled = true;
                    btnSalir.Enabled = true;
                  
                }
                else
                {

                    MessageBox.Show("Faltan Ingresar Datos");
                }

        }


        private void btnBuscar_Click(object sender, EventArgs e)
        {
            btnNuevo.Enabled = true;
            btnSalir.Enabled = true;
            txtUsuario.Enabled = false;
            ABM_Cliente.BuscarCliente bc = new ABM_Cliente.BuscarCliente();
            this.Close();
            bc.Show();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            txtNombre.Focus();
            btnGrabar.Enabled = true;
            ban = 2;
            chkMail.Enabled = true;
            lblNombre.Enabled = true;
            txtNombre.Enabled = true;
            boxDatosCliente.Enabled = true;
            btnModificar.Enabled = false;
            btnNuevo.Enabled = false;
            btnLimpiar.Enabled = true;
            btnBuscar.Enabled = false;
            btnEliminar.Enabled = false;
            
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiarDatos();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

            Conexion con1 = new Conexion();
            con1.cnn.Open();
            //Baja logica del domicilio

            string salida = abm.eliminarCliente(cbID.Text, Convert.ToInt32(txtNumeroID.Text));
            MessageBox.Show(""+salida);
            con1.cnn.Close();
            limpiarDatos();
           txtNombre.Focus();
           boxDatosCliente.Enabled = false;
           btnEliminar.Enabled = false;
           btnGrabar.Enabled = false;
           btnGrabar.Enabled = false;
           btnNuevo.Enabled = true;
           btnModificar.Enabled = false;
       }

        
        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
            if (txtNombre.Text != " ")
            {
                btnGrabar.Enabled = true;

            }
        }

        private void txtNumeroCalle_Validating(object sender, CancelEventArgs e)
        {
            if (txtNumeroCalle.Text == "")
            {

                errorProvider1.SetError(txtNumeroCalle, "No ingreso Numero de Calle");
                e.Cancel = true;

            }
            else
            {
                errorProvider1.Clear();
            }
        }

      private void chkMail_CheckedChanged(object sender, EventArgs e)
        {
            if(chkMail.Checked==true)
            {
                txtMail.Enabled = true;
            }
            else
            {
                txtMail.Enabled = false;
            }
        }
      private bool mismoID()
      {
          Conexion con = new Conexion();
          //VERIFICO SI SE REPITEN LOS IDS
          con.cnn.Open();
          string query = "SELECT 1 "+
                         "FROM LPP.CLIENTES C JOIN LPP.TIPO_DOCS T ON c.id_tipo_doc=t.tipo_cod "+
                         "WHERE C.num_doc = "+Convert.ToDecimal(txtNumeroID.Text)+" AND T.tipo_descr='"+cbID.Text+"'";
          SqlCommand command = new SqlCommand(query, con.cnn);
          SqlDataReader lector = command.ExecuteReader();
          if (lector.HasRows)
              return true;
          else
              return false;
      }
      private void limpiarDatos()
      {
          boxDatosCliente.Enabled = false;
          cbID.Text = "";
          fechaNacimiento.Value = fecha;
          lblNombre.Enabled = false;
          txtNombre.Enabled = false;
          btnLimpiar.Enabled = true;
          btnGrabar.Enabled = false;
          btnBuscar.Enabled = true;
          btnNuevo.Enabled = true;
          txtUsuario.Text = "";
          btnSalir.Enabled = true;
          txtNombre.Text = "";
          txtApellido.Text = "";
          txtLocalidad.Text = "";
          cb2.Text = "";
          txtNumeroID.Text = "";
          txtMail.Text = "";
          txtDomicilio.Text = "";
          txtNumeroCalle.Text = "";
          txtPiso.Text = "";
      }

  

    }
}
