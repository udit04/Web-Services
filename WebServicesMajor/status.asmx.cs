using System;
using System.Web;
using System.Web.Services;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.Web.Script.Serialization;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Linq;

namespace WebServicesMajor
{
    public class PassengerDetails
    {
        public string sno;
        public string bStatus;
        public string cStatus;
    }

    public class PNR
    {
        public string pnr;
        public int tno;
        public string tName;
        public string journeyDate;
        public string tclass;
        public string from;
        public string to;
        public string reservedUpto;
        public string boardingPoint;
        public int psngrCount;
        public string chartStatus;

        public PassengerDetails[] passengerDetails;
    }

    public class MovieList
    {
        public string movieName;
    }


    /// <summary>
    /// Summary description for status
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class status : System.Web.Services.WebService
    {

        [WebMethod]
        public void HotelSearch(string cityCode,string checkIn,string checkOut)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            try
            {
                string url = "http://hotelz.makemytrip.com/makemytrip/site/hotels/search/searchWithJson?city="+cityCode+"&country=IN&checkin="+checkIn+"&checkout="+checkOut+"&roomStayQualifier=1e0e&hotelId=&searchText="+cityCode+", India";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                var responseFromServer = new StreamReader(stream).ReadToEnd();
                //hotel.HotelResponse s1 = js.Deserialize<hotel.HotelResponse>(responseFromServer);

                Context.Response.Write(responseFromServer);
            }
            catch (Exception e)
            {
                Context.Response.Write(e.ToString());
            }
        }

        [WebMethod]
        public void RechargePlans(int Opt,int Cir,string Category)
        {
            StringBuilder html = new StringBuilder();
            //Table start.
            html.Append("<table border = '1'>");
            //Building the Header row.
            html.Append("<tr>");
            html.Append("<th>");html.Append("Amount");html.Append("</th>");
            html.Append("<th>"); html.Append("Detail"); html.Append("</th>");
            html.Append("<th>"); html.Append("Validity"); html.Append("</th>");
            html.Append("</tr>");

            JavaScriptSerializer js = new JavaScriptSerializer();
            RechargePlans[] rechargePlans;
            try
            {
                string url = "https://joloapi.com/api/findplan.php?userid=dummy&key=101059574097915&opt=" + Opt + "&cir=" + Cir + "&typ=" + Category + "&amt=&max=&type=json";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        var responseFromServer = new StreamReader(stream).ReadToEnd();
                        rechargePlans =  js.Deserialize<RechargePlans[]>(responseFromServer);
                        
                        for(int i=0;i<rechargePlans.Length;i++)
                        {
                            html.Append("<tr>");
                            html.Append("<td>");html.Append(rechargePlans[i].amount);html.Append("</td>");
                            html.Append("<td>"); html.Append(rechargePlans[i].Detail); html.Append("</td>");
                            html.Append("<td>"); html.Append(rechargePlans[i].Validity); html.Append("</td>");
                            html.Append("</tr>");
                        }                      
                        html.Append("</table>");
                    }
                }
                Context.Response.Write(html.ToString());
            }
            catch (Exception e)
            {
                Context.Response.Write(e.ToString());
            }
        }



        [WebMethod]
        public void LatestMoviesList()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            MovieList[] mList;
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = new HtmlDocument();
                doc = web.Load("https://www.sahinahi.com/");
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@id='latestmovies']/div[2]/div/div[1]/table/tbody/tr/td[2]/a");
                //*[@id="latestmovies"]/div[2]/div[1]/div[1]/table/tbody/tr/td[2]/a
                //*[@id="latestmovies"]/div[2]/div[2]/div[1]/table/tbody/tr/td[2]/a
                mList = new MovieList[nodes.Count];
                for (int i = 0; i < nodes.Count; i++)
                {
                    mList[i] = new MovieList();
                    mList[i].movieName = nodes[i].InnerHtml;
                }

                Context.Response.ContentType = "application/json";

                Context.Response.Write(js.Serialize(mList));
            }

            catch (Exception e)
            {
                Context.Response.Write("Service Error");

            }

        }

        [WebMethod]
        public void LatestMoviesList_2()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            MovieList[] mList;
            int i,k;
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = new HtmlDocument();
                doc = web.Load("https://www.sahinahi.com/Movies?pg=1");
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@id='movie-listing']/div/div[2]/h3/a");
                //*[@id="movie-listing"]/div[4]/div[2]/h3/a
                //*[@id="movie-listing"]/div/div[2]/div[1]/div[2]/h3/a
                //*[@id="movie-listing"]/div/div[2]/div[2]/div[2]/h3/a
                mList = new MovieList[nodes.Count*2];
                for (i = 0; i < nodes.Count; i++)
                {
                    mList[i] = new MovieList();
                    mList[i].movieName = nodes[i].InnerHtml;
                }
                k = i;
                doc = web.Load("https://www.sahinahi.com/Movies?pg=2");
                nodes = doc.DocumentNode.SelectNodes("//div[@id='movie-listing']/div/div[2]/h3/a");
                //*[@id="movie-listing"]/div/div[2]/div[1]/div[2]/h3/a
                //*[@id="movie-listing"]/div/div[2]/div[2]/div[2]/h3/a
                for (i = k; i < k+nodes.Count; i++)
                {
                    mList[i] = new MovieList();
                    mList[i].movieName = nodes[i-nodes.Count].InnerHtml;
                }

                Context.Response.ContentType = "application/json";

                Context.Response.Write(js.Serialize(mList));
            }

            catch (Exception e)
            {
                Context.Response.Write(e.ToString());
            }

        }


        [WebMethod]
        public void GetTrainSchedule(int tno)
        {
            HtmlDocument doc = new HtmlDocument();
            try
            {
                string url = "http://www.trainman.in/train/" + tno;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        var responseFromServer = new StreamReader(stream).ReadToEnd();
                        //Context.Response.Write(responseFromServer);

                        doc.LoadHtml(responseFromServer);

                        //fetching nodes and tr's and td's
                        HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[@class='master-style']/table");
                        HtmlNodeCollection nhead = doc.DocumentNode.SelectNodes("//div[@class='master-style']/table/thead/tr/th");
                        HtmlNodeCollection nrow = doc.DocumentNode.SelectNodes("//div[@class='master-style']/table/tbody/tr");
                        HtmlNodeCollection ncol = doc.DocumentNode.SelectNodes("//div[@class='master-style']/table/tbody/tr/td");
                        HtmlNodeCollection nimg = doc.DocumentNode.SelectNodes("//div[@class='master-style']/table/tbody/tr/td[@class='no-border']");

                        //fetching trainname and running days and appending it to the top of the table
                        string trainName = doc.DocumentNode.SelectSingleNode("//div[@class='master-style']/div[1]").InnerText;
                        string runsOn = doc.DocumentNode.SelectSingleNode("//div[@class='runs-on']/div").InnerText;
                        HtmlNode newNode = HtmlNode.CreateNode("<caption>" + trainName + "<br>" + runsOn + "</caption>");
                        node.PrependChild(newNode);

                        //removing cols and setting appropriate colspans
                        HtmlNode nHeadToRemove = doc.DocumentNode.SelectSingleNode("//div[@class='master-style']/table/thead/tr/th[1]");
                        nHeadToRemove.SetAttributeValue("colspan", "1");
                        foreach (HtmlNode n in nimg)
                            n.Remove();

                        //setting attributes - border etc
                        node.SetAttributeValue("style", "border: 1px solid black;border-collapse: collapse");
                        foreach (HtmlNode n in nhead)
                            n.SetAttributeValue("style", "border: 1px solid black;border-collapse: collapse");
                        foreach (HtmlNode n in nrow)
                            n.SetAttributeValue("style", "border: 1px solid black;border-collapse: collapse");
                        foreach (HtmlNode n in ncol)
                            n.SetAttributeValue("style", "border: 1px solid black;border-collapse: collapse");

                        Context.Response.Write(node.OuterHtml);
                        //return node.OuterHtml;

                    }
                }
            }
            catch (Exception e)
            {
                // return e.ToString();
            }
        }

        [WebMethod]
        public void GetPNRStatus(string sPNR)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            try
            {
                string postData = "pnrno=" + sPNR + "&submitpnr=%20%20%20%20Get%20PNR%20Status";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.pnrstatusbuzz.in");
                request.KeepAlive = true;
                request.Method = "POST";
                byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                //request.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
                //dataStream.Flush();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                string responseFromServer;
                using (StreamReader sr = new StreamReader(dataStream))
                {
                    dataStream.Flush();
                    responseFromServer = sr.ReadToEnd();
                }

                PNR pnr = new PNR();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseFromServer);
                foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//div[@id='pnrnodiv']/div[1]/table[1]"))
                {
                    foreach (HtmlNode row in table.SelectNodes("tr"))
                    {
                        HtmlNodeCollection cells = row.SelectNodes("td");
                        if (cells == null)
                            continue;

                        foreach (HtmlNode cell in cells)
                        {
                            if (cell.InnerText.Contains("PNR Number:"))
                                pnr.pnr = sPNR;
                            else if (cell.InnerText.Contains("Train Number:"))
                                int.TryParse(cell.SelectSingleNode(".//span").InnerText,out pnr.tno);
                            else if (cell.InnerText.Contains("Train Name:"))
                                pnr.tName = cell.SelectSingleNode(".//span").InnerText;
                            else if (cell.InnerText.Contains("Boarding Date (DD-MM-YYYY)"))
                                pnr.journeyDate = cell.InnerText;
                            else if (cell.InnerText.Contains("Class:"))
                                pnr.tclass = cell.SelectSingleNode(".//span").InnerText;
                            else if (cell.InnerText.Contains("From Station:"))
                                pnr.from = cell.SelectSingleNode(".//span").InnerText;
                            else if (cell.InnerText.Contains("To Station:"))
                                pnr.to = cell.SelectSingleNode(".//span").InnerText;
                            else if (cell.InnerText.Contains("Reserved Upto:"))
                                pnr.reservedUpto = cell.SelectSingleNode(".//span").InnerText;
                            else if (cell.InnerText.Contains("Boarding Point:"))
                                pnr.boardingPoint = cell.SelectSingleNode(".//span").InnerText;
                            else if (cell.InnerText.Contains("No. of Passenger:"))
                            {
                                int.TryParse(cell.SelectSingleNode(@".//span").InnerText.Trim(),out pnr.psngrCount);
                                pnr.passengerDetails = new PassengerDetails[pnr.psngrCount];
                            }
                        }
                    }
                }

                foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//div[@id='pnrnodiv']/div[1]/table[2]"))
                {
                    int i;
                    HtmlNodeCollection trnodes = table.SelectNodes("tr");
                    for (i = 1; i < trnodes.Count - 1; i++)
                    {
                        HtmlNodeCollection cells = trnodes[i].SelectNodes("td");
                        if (cells == null)
                            pnr.passengerDetails[i - 1] = null;
                        else
                        {
                            pnr.passengerDetails[i - 1] = new PassengerDetails();
                            pnr.passengerDetails[i - 1].sno = cells[0].InnerText;
                            pnr.passengerDetails[i - 1].bStatus = cells[1].InnerText;
                            pnr.passengerDetails[i - 1].cStatus = cells[2].InnerText;
                        }
                    }
                    pnr.chartStatus = trnodes[i].SelectNodes("td")[0].InnerText;
                }
                dataStream.Close();
                response.Close();

                Context.Response.ContentType = "application/json";

                Context.Response.Write(js.Serialize(pnr));
            }
            catch(Exception e)
            {
                Context.Response.Write(js.Serialize(e.ToString()));
            }
        }

    }
}
