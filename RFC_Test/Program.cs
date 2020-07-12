using SAP.Middleware.Connector;
using System;
using System.Configuration;

namespace RFC_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            RfcConfigParameters rfcConfigs = GetRfcConfigs();

            try
            {
                // 設定登入參數
                RfcDestination rfcDest = RfcDestinationManager.GetDestination(rfcConfigs);
                RfcSessionManager.BeginContext(rfcDest);

                IRfcFunction rfcReader = rfcDest.Repository.CreateFunction("ZFMM_MID_PLATFORM");

                // 建立IRfcTable 結構
                rfcReader.Invoke(rfcDest);
                IRfcStructure base_data_rt = rfcReader.GetStructure("BASE_DATA_RT");
                IRfcTable bom = rfcReader.GetTable("BOM");

                // 設定參數
                base_data_rt.SetValue("MATNR", "223000005678");
                base_data_rt.SetValue("MATTX", "測試商品");

                for (int i = 0; i < 10; i++)
                {
                    bom.Append();
                    bom[i].SetValue("IDNRK", i.ToString());
                    bom[i].SetValue("CPQTY", i);
                }

                // 輸入參數
                rfcReader.SetValue("BASE_DATA_RT", base_data_rt);
                rfcReader.SetValue("BOM", bom);

                // 執行
                rfcReader.Invoke(rfcDest);

                // 取得回傳值
                int status = rfcReader.GetInt("STATUS");
                string message = rfcReader.GetString("MSG");

                RfcSessionManager.EndContext(rfcDest);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }

        private static RfcConfigParameters GetRfcConfigs()
        {
            string user = ConfigurationManager.AppSettings["user"];
            string password = ConfigurationManager.AppSettings["password"];


            return new RfcConfigParameters()
            {
                { RfcConfigParameters.Name, "ERP" },
                { RfcConfigParameters.SystemID, "PRD" },
                { RfcConfigParameters.MessageServerHost, "10.1.101.101" },
                { RfcConfigParameters.LogonGroup, "ERP Logon" },
                { RfcConfigParameters.User, user },
                { RfcConfigParameters.Password, password },
                { RfcConfigParameters.Client, "300" },
                { RfcConfigParameters.Language, "ZF" },
                { RfcConfigParameters.Codepage, "8300" }
            };
        }
    }
}

