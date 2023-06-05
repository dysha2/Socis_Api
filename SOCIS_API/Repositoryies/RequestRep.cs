﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SOCIS_API.Repositoryies
{
    public class RequestRep : IRequestRep
    {
        private EquipmentContext _context;
        public RequestRep(EquipmentContext equipmentContext)
        {
            _context = equipmentContext;
        }
        #region Get
        public List<Request> GetMyAll(int userId)
        {
            var reqs = _context.Requests.Where(x => x.DeclarantId == userId);
            return ReqLoadData(reqs).ToList();
        }
        public List<Request> GetMyActiveAll(int userId)
        {
            var reqs = _context.Requests
                .Where(x => x.DeclarantId == userId && x.IsComplete == false);
            return ReqLoadData(reqs).ToList();
        }

        public List<Request> GetMyCompletedAll(int userId)
        {
            var reqs = _context.Requests
                .Where(x => x.DeclarantId == userId && x.IsComplete == true);
            return ReqLoadData(reqs).ToList();
        }
        public Request? GetMy(int id, int userId)
        {
            var reqs = _context.Requests.AsQueryable();
            return ReqLoadData(reqs).FirstOrDefault(x=>x.DeclarantId == userId && x.Id == id);
        }
        public List<Request> GetAll()
        {
            var reqs = _context.Requests.AsQueryable();
            return ReqLoadData(reqs).ToList();
        }

        public Request? Get(int RequestId)
        {
            var reqs = _context.Requests.AsQueryable();
            return ReqLoadData(reqs).FirstOrDefault(x => x.Id == RequestId);
        }
        public List<Request> GetMyByImpActiveAll(int userId)
        {
            var reqs = _context.Requests
                .Where(x => x.IsComplete == false && x.CurrentImplementerId == userId);
            return ReqLoadData(reqs).ToList();

        }
        public List<Request> GetMyByImpCompletedAll(int userId)
        {
            var reqs = _context.Requests
                .Where(x => x.IsComplete == true && x.CurrentImplementerId == userId);
            return ReqLoadData(reqs).ToList();
        }
        private IQueryable<Request> ReqLoadData(IQueryable<Request> reqs)
        {
          return reqs.Include(x => x.Place)
                .Include(x => x.RequestStatus)
                .Include(x => x.Declarant)
                .Select(x => new RequestDTO(x)
                {
                    Declarant = new PersonDTO(x.Declarant),
                    Place = new PlaceDTO(x.Place),
                    RequestStatus = new RequestStatusDTO(x.RequestStatus)
                });
        }
            #endregion
            #region Add
            public void AddMy(Request req, int userId)
        {
            req.DeclarantId = userId;
            req.DateTimeStart = new DateTime();
            req.IsComplete = false;
            req.DateTimeEnd = null;
            _context.Requests.Add(req);
            _context.SaveChanges();

        }
        public void Add(Request req)
        {
            req.DateTimeStart = new DateTime();
            req.IsComplete = false;
            req.DateTimeEnd = null;
            _context.Requests.Add(req);
            _context.SaveChanges();
        }
        #endregion
        #region Update
        public void UpdateMy(int reqId,Request request, int userId)
        {
            Request updateReq = _context.Requests.Find(reqId);
            if (updateReq.IsComplete) throw new Exception("Request is complete. Update banned");
            if (updateReq.DeclarantId != userId) throw new Exception("Request declarant isn`t you. Update banned");
            updateReq.Description = request.Description;
            updateReq.PlaceId = request.PlaceId;
            updateReq.IsComplete = request.IsComplete;
            _context.SaveChanges();
        }
        #endregion
        #region Delete
        public void Delete(int reqId)
        {
            throw new NotImplementedException();
        }

        
        #endregion
    }
}
