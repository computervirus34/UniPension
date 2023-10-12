using UniPension.Data;
using UniPension.IConfiguration;
using UniPension.IRepositories;
using UniPension.Models;
using UniPension.Repositories;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniPension.Controllers
{
    public class BranchController : Controller
    {
        public string perPage = ConfigurationManager.AppSettings["pageSize"];
        private readonly IUnitOfWork _unitOfWork;

        public BranchController()
        {
            _unitOfWork = new UnitOfWork();
        }

        //public BranchController(IUnitOfWork unitOfWork)
        //{
        //    this._unitOfWork = unitOfWork;
        //}
        // GET: Branch
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.CodeSortParm = sortOrder == "Code" ? "code_desc" : "Code";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var branches = from s in _unitOfWork.Branches.GetAll()
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                branches = branches.Where(s => s.branch_code.ToUpper().Contains(searchString.ToUpper())
                                       || s.branch_name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    branches = branches.OrderByDescending(s => s.branch_name);
                    break;
                case "Code":
                    branches = branches.OrderBy(s => s.branch_code);
                    break;
                case "code_desc":
                    branches = branches.OrderByDescending(s => s.branch_code);
                    break;
                default:  // Name ascending 
                    branches = branches.OrderBy(s => s.branch_code);
                    break;
            }
            int pageSize = Convert.ToInt32(perPage);
            int pageNumber = (page ?? 1);
            return View(branches.ToPagedList(pageNumber, pageSize));
        }

        // GET: Branch/Details/5
        public ActionResult Details(int id)
        {
            var branch = _unitOfWork.Branches.GetByID(id);
            return View(branch);
        }

        // GET: Branch/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Branch/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Branch/Edit/5
        public ActionResult Edit(int id)
        {
            Branch branch = _unitOfWork.Branches.GetByID(id);
            //PopulateDepartmentsDropDownList(branch.id);
            return View(branch);
        }

        // POST: Branch/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Branch branch)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _unitOfWork.Branches.Update(branch);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            //PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(branch);
        }

        // GET: Branch/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Branch/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
