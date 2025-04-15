using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrusteePortal.Models;
using TrusteePortal.NAVWS;

namespace TrusteePortal.Controllers
{
    public class BoardController : Controller
    {
        // GET: Board
        Trustee webportals = Components.ObjNav;
        string[] strLimiters = new string[] { "::" };
        string[] strLimiters2 = new string[] { "[]" };
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BoardMeetingList(string username)
   
        {
            if (Session["trusteeNo"] == null)

                return RedirectToAction("index", "login");

            username = Session["trusteeNo"].ToString();
            var BoardMeetings = new List<BoardMeeting>();
            try
            {
                // string username = Session["pensionerNo"].ToString();
                string boardMeetingList = webportals.GetBoardMeetingsAttended1(username);
                if (!string.IsNullOrEmpty(boardMeetingList))
                {
                    string[] boardMeetingListArr = boardMeetingList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string boardMeeting in boardMeetingListArr)
                    {
                        string[] response = boardMeeting.Split(strLimiters, StringSplitOptions.None);
                        //Message += 'SUCCESS' + '::' + BoardRegister."Board Meeting"  + '::' + format(BoardRegister."Posting Date") + '::' + format(BoardRegister."Meeting Type" )+ '::'+ BoardRegister.Description ;
                        BoardMeeting attendedBoardMeetings = new BoardMeeting()
                        {
                            //Code = response[1].Trim(),
                            MeetingNo = response[1].Trim(),
                            Date = response[2].Trim(),
                            Type = response[3].Trim(),
                            Description = response[4].Trim(),
                            


                        };
                        BoardMeetings.Add(attendedBoardMeetings);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("index", "dashboard");
            }
            return View(BoardMeetings);


        }
        public ActionResult AssignedBoardList(string username)

        {
            if (Session["trusteeNo"] == null)

                return RedirectToAction("index", "login");

            username = Session["trusteeNo"].ToString();
            var Board = new List<Board>();
            try
            {
                // string username = Session["pensionerNo"].ToString();
                string assignedBoardList = webportals.GetAssignedBoards(username);
                if (!string.IsNullOrEmpty(assignedBoardList))
                {
                    string[] assignedBoardListArr = assignedBoardList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string assignedBoard in assignedBoardListArr)
                    {
                        string[] response = assignedBoard.Split(strLimiters, StringSplitOptions.None);
                        
                        Board assignedBoards = new Board()
                        {
                            Code = response[1].Trim(),
                            Name = response[2].Trim(),
                           // Description = response[3].Trim(),


                        };
                        Board.Add(assignedBoards);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("index", "dashboard");
            }
            return View(Board);


        }
    }
}