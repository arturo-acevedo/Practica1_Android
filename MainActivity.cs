using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.IO;
using Java.Net;
using Kotlin.Text;
using System.Net;
using Javax.Net.Ssl;

namespace Practica1_Android
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        EditText? txtNombre, txtDomicilio, txtCorreo, txtEdad, txtSaldo, txtID;
        TextView? txtUrlresultado, txtContenidoresultado;
        Button? btnGuardar, btnConsultar, btnXml,btnSql,btnSqlite;
        String? Nombre, Domicilio, Correo,Resultado;
        
        int Edad, ID;
        double Saldo;
        HttpClient cliente = new HttpClient();

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            txtID = FindViewById<EditText>(Resource.Id.txtid);
            txtNombre = FindViewById<EditText>(Resource.Id.txtnombre);
            txtDomicilio = FindViewById<EditText>(Resource.Id.txtdomicilio);
            txtCorreo = FindViewById<EditText>(Resource.Id.txtcorreo);  
            txtEdad = FindViewById<EditText>(Resource.Id.txtedad);
            txtSaldo = FindViewById<EditText>(Resource.Id.txtsaldo);
            btnGuardar = FindViewById<Button>(Resource.Id.btnguardar);
            btnConsultar = FindViewById<Button>(Resource.Id.btnconsultar);
            btnXml = FindViewById<Button>(Resource.Id.btnxml);
            btnSql = FindViewById<Button>(Resource.Id.btnsql);
            btnSqlite = FindViewById<Button>(Resource.Id.btnsqlite);
            txtUrlresultado = FindViewById<TextView>(Resource.Id.txturlresultado);
            txtContenidoresultado = FindViewById<TextView>(Resource.Id.txtcontenidoresultado);

            btnGuardar.Click += delegate
            {
                var DC = new Datos();
                //Guardar la informacion en XML
                try
                {
                    DC.Id = int.Parse(txtID.Text);
                    DC.Nombre = txtNombre.Text;
                    DC.Domicilio = txtDomicilio.Text;
                    DC.Correo = txtCorreo.Text;
                    DC.Edad = int.Parse(txtEdad.Text);
                    DC.Saldo = int.Parse(txtSaldo.Text);

                    var serializador = new XmlSerializer(typeof(Datos));
                    var Escritura = new StreamWriter (Path.Combine(System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), txtID.Text + ".xml"));  
                    serializador.Serialize(Escritura, DC);
                    Escritura.Close();

                    Toast.MakeText(this, "Archivo XML Guardado Correctamente", ToastLength.Long).Show();
                    
                    ;

                }
                catch (Exception ex)
                {

                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                };
                // Guardar la informacion en SQL
                try
                {

                    Nombre = txtNombre.Text;
                    Domicilio = txtDomicilio.Text;
                    Correo = txtCorreo.Text;
                    Edad = int.Parse(txtEdad.Text);
                    Saldo = int.Parse(txtSaldo.Text);

                    var API = "http://172.23.1.71:85/Principal/AlmacenarSQLServer?Nombre=" +
                    Nombre + "&Domicilio=" + Domicilio + "&Correo=" + Correo +
                    "&Edad=" + Edad + "&Saldo=" + Saldo;
                    HttpResponseMessage response = cliente.GetAsync(API).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var resultado = response.Content.ReadAsStringAsync().Result;

                        Toast.MakeText(this, resultado.ToString(), ToastLength.Long).Show();
                                            
                    }


                }
                catch (Exception ex)
                {

                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
                //Guardar la informacion en SQLite
                try
                {
                    var csql = new ClaseSQLite();
                    csql.nombre = txtNombre.Text;
                    csql.domicilio = txtDomicilio.Text;
                    csql.correo = txtCorreo.Text;
                    csql.edad = int.Parse(txtEdad.Text);
                    csql.saldo = double.Parse(txtSaldo.Text);
                    csql.ConexionBase();
                    if ((csql.IngresarDatos (csql.nombre, csql.domicilio, csql.correo, csql.edad, csql.saldo)) == true)
                    {
                        Toast.MakeText(this,"Guardado Correctamente en SQLite",ToastLength.Long).Show();

                        txtID.Text = "Id";
                        txtNombre.Text = "Nombre";
                        txtDomicilio.Text = "Domicilio";
                        txtCorreo.Text = "Correo";
                        txtEdad.Text = "Edad";
                        txtSaldo.Text = "Saldo";
                    }
                    else
                    {
                        Toast.MakeText(this,
                            "No guardado", ToastLength.Long).Show();
                    }
                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }

            };

            btnConsultar.Click += async delegate
            {
                var DC = new Datos();
                try
                {
                    DC.Id = int.Parse(txtID.Text);
                    var serializador = new XmlSerializer(typeof(Datos));
                    var Lectura = new StreamReader(Path.Combine(System.Environment.GetFolderPath (Environment.SpecialFolder.Personal), txtID.Text + ".xml"));
                    var datos = (Datos)serializador.Deserialize(Lectura);
                    Lectura.Close();
                    txtNombre.Text = datos.Nombre;
                    txtDomicilio.Text = datos.Domicilio;
                    txtCorreo.Text = datos.Correo;
                    txtEdad.Text = datos.Edad.ToString();
                    txtSaldo.Text = datos.Saldo.ToString();

                    string xmlContent = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), txtID.Text + ".xml"));
                    string xmlRuta = (Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), txtID.Text + ".xml"));
                    //txtResultado.Text = ("URL:"+ ruta + "Contenido:" + xmlContent);
                    Toast.MakeText(this,("URL"+xmlRuta),ToastLength.Long).Show();
                    Toast.MakeText(this,("CONTENIDO:"+xmlContent), ToastLength.Long).Show();

                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }

                try
                {
                    ID = int.Parse(txtID.Text);
                    var API = "http://172.23.1.71:85/Principal/ConsultarSQLServer?ID=" + ID;
                    var json = await TraerDatos(API);
                    foreach (var repo in json)
                    {
                        txtNombre.Text = repo.Nombre;
                        txtDomicilio.Text = repo.Domicilio;
                        txtCorreo.Text = repo.Correo;
                        txtEdad.Text = repo.Edad.ToString();
                        txtSaldo.Text = repo.Saldo.ToString();
                                               
                    }
                    
                    string sqlRuta = (API);
                    Toast.MakeText(this, ("URL:" + sqlRuta), ToastLength.Long).Show();

                    WebClient client = new WebClient();
                    string sqlContent = client.DownloadString(sqlRuta);
                    Toast.MakeText(this, ("CONTENIDO:" + sqlContent), ToastLength.Long).Show();

                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }

                try
                {
                    var csql = new ClaseSQLite();
                    csql.ID = int.Parse(txtID.Text);
                    csql.Buscar(csql.ID);
                    txtNombre.Text = csql.nombre;
                    txtDomicilio.Text = csql.domicilio;
                    txtCorreo.Text = csql.correo;
                    txtEdad.Text = csql.edad.ToString();
                    txtSaldo.Text = csql.saldo.ToString();


                    


                    string sqliteContent = ("ID:"+" "+csql.ID+" "+"Nombre:"+" "+csql.nombre+" "+"Domicilio:"+" "+csql.domicilio+" "+"Correo:"+" "+csql.correo+" "+"Edad:"+" "+csql.edad+" "+"Saldo:"+" "+csql.saldo).ToString();
                    Toast.MakeText(this, ("CONTENIDO:" + sqliteContent), ToastLength.Long).Show();



                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }




            };

            btnXml.Click += delegate {
                var DC = new Datos();
                try
                {
                    DC.Id = int.Parse(txtID.Text);
                    var serializador = new XmlSerializer(typeof(Datos));
                    var Lectura = new StreamReader(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), txtID.Text + ".xml"));
                    var datos = (Datos)serializador.Deserialize(Lectura);
                    Lectura.Close();
                    txtNombre.Text = datos.Nombre;
                    txtDomicilio.Text = datos.Domicilio;
                    txtCorreo.Text = datos.Correo;
                    txtEdad.Text = datos.Edad.ToString();
                    txtSaldo.Text = datos.Saldo.ToString();
                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            btnSql.Click += async delegate 
            {
                try
                {
                    ID = int.Parse(txtID.Text);
                    var API = "http://172.23.1.71:85/Principal/ConsultarSQLServer?ID=" + ID;
                    var json = await TraerDatos(API);
                    foreach (var repo in json)
                    {
                        txtNombre.Text = repo.Nombre;
                        txtDomicilio.Text = repo.Domicilio;
                        txtCorreo.Text = repo.Correo;
                        txtEdad.Text = repo.Edad.ToString();
                        txtSaldo.Text = repo.Saldo.ToString();
                    }
                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            btnSqlite.Click += delegate 
            {
                try
                {
                    var csql = new ClaseSQLite();
                    csql.ID = int.Parse(txtID.Text);
                    csql.Buscar(csql.ID);
                    txtNombre.Text = csql.nombre;
                    txtDomicilio.Text = csql.domicilio;
                    txtCorreo.Text = csql.correo;
                    txtEdad.Text = csql.edad.ToString();
                    txtSaldo.Text = csql.saldo.ToString();
                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };



        }
        private async Task<List<Datos>> TraerDatos(string API)
        {
            cliente.DefaultRequestHeaders.Accept.Clear();
            var streamTask = cliente.GetStreamAsync(API);
            var repositorio = await
                System.Text.Json.JsonSerializer.DeserializeAsync<List<Datos>>(await streamTask);
            return repositorio;
        }
    }


    public class Datos
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("domicilio")]
        public string Domicilio { get; set; }

        [JsonPropertyName("correo")]
        public string Correo { get; set; }

        [JsonPropertyName("edad")]
        public int Edad { get; set; }

        [JsonPropertyName("saldo")]
        public double Saldo { get; set; }



    }
}
