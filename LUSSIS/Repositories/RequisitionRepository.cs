﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LUSSIS.Models;
using LUSSIS.Models.WebDTO;
using LUSSIS.Repositories.Interface;

namespace LUSSIS.Repositories
{
    //still working on it. --- xiaowen

    public class RequisitionRepository : Repository<Requisition, int>, IRequisitionRepository
    {
        DisbursementRepository disRepo = new DisbursementRepository();

        //consolidate requisition details(group by item) + disbursement table status = disbursement details(group by item)
        public List<RetrievalItemDTO> GetConsolidatedRequisition()
        {
            List<RetrievalItemDTO> itemsToRetrieve = new List<RetrievalItemDTO>();
            ConsolidateRequisitionQty(itemsToRetrieve, GetRequisitionDetailsByStatus("approved"));
            ConsolidateRemainingQty(itemsToRetrieve, GetUnfullfilledDisDetailList());
            return itemsToRetrieve;
        }

        /*
         * helper method to consolidate requisitions for one item into one RetrievalItemDTO
        */
        private void ConsolidateRequisitionQty(List<RetrievalItemDTO> targetRetreivalList, List<RequisitionDetail> requisitionDetailList)
        {
            //group RequisitionDetail list by item: e.g.: List<ReqDetail>-for-pen, List<ReqDetail>-for-Paper, and store these lists in List:
            List<List<RequisitionDetail>> groupedReqList = requisitionDetailList.GroupBy(rd=>rd.ItemNum).Select(grp=>grp.ToList()).ToList();

            //each list merge into ONE RetrievalItemDTO. e.g.: List<ReqDetail>-for-pen to be converted into ONE RetrievalItemDTO. 
            foreach (List<RequisitionDetail> perItemReqList in groupedReqList)
            {
                Stationery stat = perItemReqList.First().Stationery;
                RetrievalItemDTO dto = GetSameStatOrAddNewDTO(targetRetreivalList, stat);

                foreach (RequisitionDetail rd in perItemReqList)
                {
                   dto.RequestedQty += rd.Quantity;
                }
            }
        }

        /*
         * helper method to consolidate unfullfilled Disbursements for one item into one RetrievalItemDTO
        */
        private void ConsolidateRemainingQty(List<RetrievalItemDTO> targetRetreivalList, List<DisbursementDetail> unfullfilledDisDetailList)
        {
            List<List<DisbursementDetail>> groupedDisList = unfullfilledDisDetailList.GroupBy(rd => rd.ItemNum).Select(grp => grp.ToList()).ToList();

            //each list merge into ONE RetrievalItemDTO. e.g.: List<DisDetail>-for-pen to be converted into ONE RetrievalItemDTO. 
            foreach (List<DisbursementDetail> perItemDisList in groupedDisList)
            {
                Stationery stat = perItemDisList.First().Stationery;
                RetrievalItemDTO dto = GetSameStatOrAddNewDTO(targetRetreivalList, stat);

                foreach (DisbursementDetail dd in perItemDisList)
                {
                    //dto.RequestedQty += dd.RequestedQty - dd.ActualQty;
                    dto.RequestedQty += (dd.RequestedQty > dd.ActualQty? dd.RequestedQty - dd.ActualQty: 0);
                }
            }
        }

        /*
         * helper method
         * check if a stat's equivlent DTO exist in the target DTO list
         * exist? return that DTO : create new DTO and add it to list
         */
        private RetrievalItemDTO GetSameStatOrAddNewDTO(List<RetrievalItemDTO> targetRetreivalList, Stationery stat)
        {
            //if yes, take the DTO out and use it
            if (targetRetreivalList.Count > 0)
            {
                foreach (RetrievalItemDTO dto in targetRetreivalList)
                {
                    if (dto.ItemNum == stat.ItemNum)
                    {
                        return dto;
                    }
                }
            }
            //if not, instantiate a new DTO, add to the list and use it instead
            RetrievalItemDTO newDto = convertStatoDTO(stat);
            targetRetreivalList.Add(newDto);
            return newDto;
        }


        public List<Requisition> GetRequisitionsByStatus(string status)
        {
            return LUSSISContext.Requisitions.Where(r => r.Status == status).ToList();
        }

        public List<RequisitionDetail> GetRequisitionDetailsByStatus(string status)
        {
            return LUSSISContext.RequisitionDetails.Where(r => r.Requisition.Status == status).ToList();
        }

        public List<DisbursementDetail> GetUnfullfilledDisDetailList()
        {
            List<DisbursementDetail> unfullfilledDisList = disRepo.GetDisbursementDetailsByStatus("unfullfilled");
            return unfullfilledDisList.Where(d => (d.RequestedQty - d.ActualQty) > 0).ToList();
        }

        //public as might need it in Disbursement later
        public RetrievalItemDTO convertStatoDTO(Stationery s)
        {
            return new RetrievalItemDTO()
            {
                ItemNum = s.ItemNum,
                BinNum = s.BinNum,
                Description = s.Description,
                UnitOfMeasure = s.UnitOfMeasure,
                AvailableQty = s.AvailableQty,
                RequestedQty = 0,
                RemainingQty = 0
            };
        }

    }
}