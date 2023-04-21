using MTC.WebApp.BackOffice.Helpers;
using MTCenter.GRPC.GDBBO.DBProtos;
using System;

namespace MTC.WebApp.BackOffice.DataManager
{
    public class SoporteManager
    {
        private DBService _client;
        public SoporteManager(DBData.DBDataClient client)
        {
            this._client = new DBService(client);

        }

        #region Administrar Clientes

        public DataRequestReply getUsoCFDI()
        {
            DataRequestModel data = new DataRequestModel();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                data.ProcedureName = "sp_getCatalogo_UsoCFDI";
                data.TransactionDb = TransactionDb.Select;

                reply = _client.GetData(data);

            }
            catch (Exception ex)
            {

                reply.CodigoRespuesta = -109;
                reply.MensajeRespuesta = ex.Message;

            }

            return reply;
        }

        #endregion
    }
}
