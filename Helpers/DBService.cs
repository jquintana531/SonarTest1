using MTCenter.GRPC.GDBBO.DBProtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Helpers
{
    public class DBService : DBData.DBDataClient
    {
        private readonly DBData.DBDataClient _client;

        public DBService(DBData.DBDataClient client)
        {

            _client = client;
        }

        public DataRequestReply GetData(DataRequestModel parameters)
        {
            DataRequestReply response = new DataRequestReply();

            try
            {

                response =  _client.GetData(parameters);
            }
            catch (Exception ex)
            {

                response.CodigoRespuesta = 109;
                response.MensajeRespuesta = ex.Message;
            }

            return response;
        }


    }
}
