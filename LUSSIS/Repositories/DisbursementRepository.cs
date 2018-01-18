﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Models;

namespace LUSSIS.Repositories
{
    public class DisbursementRepository : Repository<Disbursement, int>
    {
        public Disbursement GetByDateAndDeptCode(DateTime nowDate, string deptCode)
        {
            try
            {
                return LUSSISContext.Disbursements.First(x => x.CollectionDate > nowDate && x.DeptCode == deptCode);
            }
            catch
            {
                return null;
            }
        }

        public CollectionPoint GetCollectionPointByDisbursement(Disbursement disbursement)
        {
            return LUSSISContext.CollectionPoints.First(y => y.CollectionPointId == disbursement.CollectionPointId);
        }

        public CollectionPoint GetCollectionPointByDeptCode(string deptCode)
        {
            Department d = new Department();
            d = LUSSISContext.Departments.First(z => z.DeptCode == deptCode);
            return LUSSISContext.CollectionPoints.First(x => x.CollectionPointId == d.CollectionPointId);
        }

        public List<DisbursementDetail> GetDisbursementDetails(Disbursement disbursement)
        {
            return LUSSISContext.DisbursementDetails.Where(x => x.DisbursementId == disbursement.DisbursementId).ToList();
        }
        public List<DisbursementDetail> GetDisbursementDetailsByStatus(string status)
        {
            return LUSSISContext.DisbursementDetails.Where(x => x.Disbursement.Status == status).ToList();
        }
        public List<DisbursementDetail> GetUnfullfilledDisDetailList()
        {
            return LUSSISContext.DisbursementDetails.Where(d => (d.RequestedQty - d.ActualQty) > 0).ToList();
        }
    }
}