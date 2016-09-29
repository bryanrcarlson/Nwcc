using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nrcs.Nwcc.ReportGenerator
{
    public class DataGrabber
    {
        private Uri baseUrl;
        private const string UNITS = "metric";
        private const string SINGLE_STATION = "customSingleStationReport";

        public DataGrabber()
        {
            baseUrl = new Uri("http://wcc.sc.egov.usda.gov/reportGenerator");
        }

        /// <summary>
        /// <note>Only returns data in CSV format</note>
        /// <note>Does not support "Functions", returns VALUE only (no Sum, Mean, etc)</note>
        /// <note>Is a dumb function, little to no error checking</note>
        /// </summary>
        /// <param name="stationId">Station ID with state and station code, example: 2198:WA:SCAN</param>
        /// <param name="startDate">First date of returned data</param>
        /// <param name="endDate">Last date of returned data</param>
        /// <returns></returns>
        public string GetHourly(
            string stationId,
            //DataFormatEncoder dataFormat,
            DateTime startDate,
            DateTime endDate,
            List<DataColumns> dataColumns)
        {
            StringBuilder urlString = new StringBuilder(baseUrl.ToString());
            urlString.Append("/" + DataFormatEncoder.CSV);
            urlString.Append("/" + SINGLE_STATION + "," + UNITS);
            urlString.Append("/hourly");
            urlString.Append("/" + stationId);
            urlString.Append("/" +
                startDate.ToString("yyyy-MM-dd")
                + "," +
                endDate.ToString("yyyy-MM-dd"));

            urlString.Append("/");
            for(int i = 0; i < dataColumns.Count; i++)
            {
                urlString.Append(dataColumns[i].ToString() + "::value");
                if(i < dataColumns.Count - 1)
                {
                    urlString.Append(",");
                }
            }

            string data = "";

            Task.Run(async () =>
            {
                data = await getData(urlString.ToString());
            }).Wait();

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">Complete url</param>
        /// <returns>CSV string</returns>
        private async Task<string> getData(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/html"));

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                return content;
            }
        }
    }
}
