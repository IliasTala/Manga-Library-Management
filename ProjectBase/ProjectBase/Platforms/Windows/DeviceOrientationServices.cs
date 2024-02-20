using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
using System.Collections;

namespace ProjectBase.Services
{
    public partial class DeviceOrientationServices
    {
        SerialPort mySerialPort;
        public QueueBuffer SerialBuffer;

        // Configuration du scanner
        public partial void ConfigureScanner()
        {
            // Initialisation du buffer série
            this.SerialBuffer = new QueueBuffer();
            this.mySerialPort = new SerialPort();
            string ComPort = "COM3"; // Port série à utiliser

            if (ComPort != "")
            {
                // Configuration des paramètres du port série
                mySerialPort.PortName = ComPort;
                mySerialPort.BaudRate = 9600;
                mySerialPort.Parity = Parity.None;
                mySerialPort.DataBits = 8;
                mySerialPort.StopBits = StopBits.One;

                mySerialPort.ReadTimeout = 1000;
                mySerialPort.WriteTimeout = 1000;

                // Gestionnaire d'événements pour la réception des données
                mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataHandler);

                try
                {
                    mySerialPort.Open(); // Ouverture du port série
                }
                catch (Exception ex)
                {
                    Shell.Current.DisplayAlert("Error!", ex.Message, "OK"); // Affichage d'une alerte en cas d'erreur
                }
            }
        }

        // Gestionnaire d'événements pour la réception des données
        private void DataHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = "";

            data = sp.ReadTo("\r"); // Lecture des données jusqu'au caractère de fin de ligne ("\r")

            SerialBuffer.Enqueue(data); // Ajout des données au buffer série
        }
    }
}
