using Application.Business.UnitOfWork;
using Domain.DTO;
using Domain.Entities;
using Domain.Helper;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AQAcademy_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    // /PlayerMemberShip/
    [ApiController]
    public class PlayerMembershipController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly PlayerDataContext _Context;

        public PlayerMembershipController(IUnitOfWork UOW, PlayerDataContext context)
        {
            _UnitOfWork = UOW;

            _Context = context;
        }     
        [Route("GetMemberById")]
        [HttpGet]
        public async Task<ActionResult<PlayerMemberShip>> GetMember([FromQuery] int id)
        {
            var Member = _UnitOfWork.PlayerMembership.FindById(id);


            return Member;
        }
      
        [HttpDelete]
        public async Task<IActionResult> DeleteMember([FromQuery] int id)
        {
            try
            {
                //Everytime a player is created its user attendance with checkin and playerhistory is created hence deleting them with player deletion
                List<UserAttendance> playerAttendance = _Context.UserAttendance.Where(a => a.PlayerId == id && a.IsActive == true).ToList();
                var packageHistory = _Context.PackageHistory.Where(ph => ph.PlayerId == id && ph.IsActive == true).OrderBy(x => x.CreatedAt).FirstOrDefault();
                var member = _UnitOfWork.PlayerMembership.FindById(id);
                member.IsActive = false;
                member.DeletedAt = Helpers.GetCurrentDateTime();

                if (packageHistory != null)
                {
                    packageHistory.IsActive = false;
                    packageHistory.DeletedAt = Helpers.GetCurrentDateTime();
                    // packageHistory.DeletedBy = userId;
                }
                if (playerAttendance.Count > 0 || playerAttendance != null)
                {
                    foreach (var item in playerAttendance)
                    {

                        item.IsActive = false;
                        item.DeletedAt = Helpers.GetCurrentDateTime();
                        //playerAttendance.DeletedBy = userId;
                        _UnitOfWork.UserAttendance.Update(item);
                    }
                }
                _UnitOfWork.PlayerMembership.Update(member);
                _UnitOfWork.PackageHistory.Update(packageHistory);
                await _UnitOfWork.SaveDbChanges();
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
            return NoContent();
        }

        [Route("GetPlayerCode")]
        [HttpGet]
        public string GetPlayerCode()
        {
            string RC = _UnitOfWork.PlayerMembership.PlayerCode();
            return RC;
        }
        [Route("GetPlayerById")]
        [HttpGet]
        public async Task<PlayerMemberShip> GetPlayer(int id)
        {
            var Player = await _UnitOfWork.PlayerMembership.GetPlayerInfo(id);
            //if (Player == null)
            //{
            //    return NotFound();
            //}
            return Player;
        }

        [Route("GetExpiredPlayers")]
        [HttpGet]
        public List<int>? GetExpiredPlayers()
        {
            List<int>? ExpiredMembership = _UnitOfWork.UserAttendance.GetExpiredMembershipCount().playerIds?.ToList();

            return ExpiredMembership;
        }
        [Route("GetExpiringPlayers")]
        [HttpGet]
        public List<int>? GetExpiringPlayers()
        {
            List<int>? ExpiringMembership = _UnitOfWork.UserAttendance.GetSoonExpiringMembershipCout().playerIds?.ToList();

            return ExpiringMembership;
        }
    }
}
