﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using LUSSIS.Models;
using LUSSIS.Models.WebAPI;
using LUSSIS.Repositories;

namespace LUSSIS.Controllers.WebAPI
{
    public class RequisitionsController : ApiController
    {
        private readonly RequisitionRepository _repo = new RequisitionRepository();

        // GET: api/Requisitions/COMM/?status=pending&empnum=77
        //        [ResponseType(typeof(RequisitionDTO))]
        //        public async Task<IHttpActionResult> GetRequisition(string dept, [FromUri]string status, [FromUri]int empnum)
        //        {
        //            var reqList = repo.GetRequisitionsByReferences(dept, status, empnum);
        //            var list = repo.GetRequisitionsByStatus(status);
        //            
        //        }

        //GET: api/Requisitions/
        [Route("api/Requisitions/{dept}")]
        public IEnumerable<RequisitionDTO> GetPending(string dept)
        {
            var list = _repo.GetPendingListForHead(dept).ToList();

            var result = list.Select(item => new RequisitionDTO()
            {
                RequisitionId = item.RequisitionId,
                RequisitionEmp = item.RequisitionEmployee.FirstName + " " + item.RequisitionEmployee.LastName,
                RequisitionDate = item.RequisitionDate,
                ApprovalEmp = item.ApprovalEmpNum != null ? item.ApprovalEmployee.FirstName + " " + item.ApprovalEmployee.LastName : null,
                ApprovalRemarks = item.ApprovalRemarks != null ? item.ApprovalRemarks : null,
                RequestRemarks = item.RequestRemarks != null ? item.RequestRemarks : null,
                RequisitionDetails = item.RequisitionDetails.Select(detail => new RequisitionDetailDTO()
                {
                    Description = detail.Stationery.Description,
                    UnitOfMeasure = detail.Stationery.UnitOfMeasure,
                    Quantity = detail.Quantity
                })
            });

            return result;
        }

        [HttpPost]
        [Route("api/Requisitions/Process")]
        public async Task<IHttpActionResult> Process(int empnum, string status, [FromBody]RequisitionDTO requisition)
        {
            var req = await _repo.GetByIdAsync(requisition.RequisitionId);
            req.ApprovalEmpNum = empnum;
            req.ApprovalRemarks = requisition.ApprovalRemarks;
            req.ApprovalDate = DateTime.Today;
            req.Status = status;

            await _repo.UpdateAsync(req);

            return Ok("Updated");
        }
    }
}