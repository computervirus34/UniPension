using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UniPension.Data;
using UniPension.IConfiguration;
using UniPension.Models;

namespace UniPension.Controllers
{
    public class PartnerController : ApiController
    {
        // GET: api/Partner
        private readonly IUnitOfWork _unitOfWork;

        public PartnerController()
        {
            _unitOfWork = new UnitOfWork();
        }
        public PartnerController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        // POST: api/Partner
        [HttpPost]
        [Route("api/Partner/VerifyTransaction")]
        public HttpResponseMessage Post([FromBody]CallBackModel callBackModel)
        {
            CallBackResponse callResponse = new CallBackResponse();
            try
            {
                var isExists = _unitOfWork.PensionPayment.GetByTransactionID(callBackModel);

                if (isExists != null)
                {
                    callResponse.code = "200";
                    callResponse.message = "Success";
                    callResponse.TransactionId = callBackModel.TransactionId;
                    callResponse.Payment_Ref_No = isExists.PaymentRefNo;
                    callResponse.PaymentBatchNo = isExists.CBSBatchNo;
                    
                    isExists.CallBackSuccess = "Y";
                    isExists.LastCallBackTime = DateTime.Now;
                    _unitOfWork.Save();

                    return Request.CreateResponse(HttpStatusCode.OK,callResponse);
                }
                else
                {
                    callResponse.code = "403";
                    callResponse.message = "No Transaction Found with this transaction ID and Ref No!";
                    callResponse.TransactionId = callBackModel.TransactionId;
                    callResponse.Payment_Ref_No = "";
                    callResponse.PaymentBatchNo = "";

                    return Request.CreateResponse(HttpStatusCode.BadRequest, callResponse);
                }

            }catch(Exception ex)
            {
                callResponse.code = "500";
                callResponse.message = ex.Message;
                callResponse.TransactionId = callBackModel.TransactionId;
                callResponse.Payment_Ref_No = "";
                callResponse.PaymentBatchNo = "";

                return Request.CreateResponse(HttpStatusCode.BadRequest, callResponse);
            }
        }
    }
}
