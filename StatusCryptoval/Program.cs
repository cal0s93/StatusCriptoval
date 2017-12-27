using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Net;

namespace StatusCryptoval
{
    class Program
    {


        public static DataSet wallet = new DataSet();
        public static DataSet apival = new DataSet();
        protected static int origRow;
        protected static int origCol;
        protected static void WriteAt(string s, int x, int y)
        {
            try
            {
                Console.SetCursorPosition(origCol + x, origRow + y);
                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        static void GetExcangeRate_CRYPTOCOMPARE(bool silent)
        {
            int i = 0;
            int n = 0;

            foreach (DataRow row in wallet.Tables[0].Rows)
            {
                string json;
                try
                {
                    string url = "https://min-api.cryptocompare.com/data/price?fsym=" + row.ItemArray[1].ToString() + "&tsyms=BTC,USD,EUR";
                    json = new WebClient().DownloadString(url);
                    JObject o = JObject.Parse(json);
                    wallet.Tables[0].Rows[i][3] = o["EUR"];

                }
                catch
                {
                    if (silent != true)
                    {
                        json = "";
                        Console.WriteLine("Unable to get te price of " + wallet.Tables[0].Rows[i][1] + " insert or skip");
                        json = Console.ReadLine();
                        int.TryParse(json, out n);
                        if (json != "")
                            wallet.Tables[0].Rows[i][3] = json;
                    }
                }

            i++;
        }
    }

        static void GetExcangeRate_CRYPTONATOR(bool silent)
        {
            int i = 0;
            int n = 0;
            
            string to = "EUR";
            foreach (DataRow row in wallet.Tables[0].Rows)
            {
                string json;
                try
                {
                    string url = "https://api.cryptonator.com/api/ticker/" + row.ItemArray[1].ToString() + "-" + to;
                    json = new WebClient().DownloadString(url);
                    JObject o = JObject.Parse(json);
                    wallet.Tables[0].Rows[i][3] = o["ticker"]["price"];

                }
                catch
                {
                    if (silent !=true)
                    {
                        json = "";
                        Console.WriteLine("Unable to get te price of " + wallet.Tables[0].Rows[i][1] + " insert or skip");
                        json = Console.ReadLine();
                        int.TryParse(json, out n);
                        if (json != "")
                            wallet.Tables[0].Rows[i][3] = json;
                    }
                }

                i++;
            }
        }

        static void Main(string[] args)
        {
            xmlreadconf();

            GetExcangeRate_CRYPTOCOMPARE(true);
            
            origRow = Console.CursorTop;
            origCol = Console.CursorLeft;
            
            int i = 0;
            int i2 = 0;
            int n = 0;
            
            while (i < 1)
            {
                decimal aux_tot = 0;
                Console.Clear();
                foreach (DataRow row in wallet.Tables[0].Rows)
                {
                    i2 = 0;
                    foreach (var col in row.ItemArray)
                    {
                        WriteAt(col.ToString(), i2 + 2, i);
                        i2 = i2 + 20;
                    }
                    
                    decimal aux_n1 = Convert.ToDecimal(row[2].ToString().Replace(".",",")) ;
                
                    decimal aux_n2 = Convert.ToDecimal(row[3].ToString().Replace(".", ","));
                    
                    
                    
                    aux_n1 = aux_n1 * aux_n2;
                    aux_tot = aux_tot +aux_n1; 
                    WriteAt(aux_n1.ToString("F"), i2, i);
                    i++;
                    WriteAt(i.ToString(), 0, i - 1);
                }
                
                WriteAt("Totale euro: " + aux_tot.ToString(), i2 -13 , i + 1);

                i = 0;

                string aux_s;

                WriteAt("1)Update excange rate 2)Add wallet 3)Edit wallet balance 9)Exit", 0, 28-i);
                
                WriteAt("Select: ", 0, 29-i);

                do
                {
                    aux_s = Console.ReadLine();
                } while (int.TryParse(aux_s, out n) == false);
                switch (n)
                {
                    case 1:

                        GetExcangeRate_CRYPTOCOMPARE(false);
                        xmlwriteconf();
                        break;
                    case 2:
                        xmladdrow(wallet);
                        xmlwriteconf();
                        break;
                    case 3:
                        EditRow(wallet);
                        xmlwriteconf();
                        break;

                    case 9:
                        i = 10;
                        break;
                }


            }
        }

        static void xmlreadconf()
        {
            try
            {
                
                //apival.ReadXml(Directory.GetCurrentDirectory() + @"\apival.xml");
                wallet.ReadXml(Directory.GetCurrentDirectory() + @"\wallet.xml");
                Console.WriteLine("XML data read successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        static void xmladdrow(DataSet data)
        {
            int i = 0;
            Console.Clear();
            DataRow row = data.Tables[0].NewRow();
            foreach (var col in row.ItemArray)
            {
                Console.WriteLine(data.Tables[0].Rows[0].Table.Columns[i]);
                row[i] = Console.ReadLine();
                i++;
            }

            data.Tables[0].Rows.Add(row);

        }

        static void EditRow(DataSet data)
        {


            string aux_s;
            int n = 0;
            int i = 0;
            do
            {
                i = 0;
                int i2 = 0;
                Console.Clear();
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    i2 = 0;
                    foreach (var col in row.ItemArray)
                    {
                        WriteAt(col.ToString(), i2 + 2, i);
                        i2 = i2 + 20;
                    }
                    i++;
                    WriteAt(i.ToString(), 0, i - 1);
                }
                WriteAt("Edit:", 0, i);
                aux_s = Console.ReadLine();
            } while (int.TryParse(aux_s, out n) == false || n > i);
            i = 0;
           
                Console.WriteLine("Set : " + data.Tables[0].Rows[n - 1].Table.Columns[2]+" "+ data.Tables[0].Rows[n - 1][0]);
                
                wallet.Tables[0].Rows[n - 1][2] = Console.ReadLine();
        }

        static void xmlwriteconf()
        {
            try
            {
                wallet.WriteXml(Directory.GetCurrentDirectory() + @"\wallet.xml");
                Console.WriteLine("XML data written successfully to " + Directory.GetCurrentDirectory() + @"\wallet.xml");
               // apival.WriteXml(Directory.GetCurrentDirectory() + @"\apival.xml");
               // Console.WriteLine("XML data written successfully to " + Directory.GetCurrentDirectory() + @"\apival.xml");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

        }
    }
}
