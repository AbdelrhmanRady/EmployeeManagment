using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Security;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnviroment;
        private readonly ILogger<HomeController> logger;
        private readonly IDataProtector protector;
        public HomeController(IEmployeeRepository employeeRepository, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnviroment,ILogger<HomeController> logger
            ,IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnviroment = hostingEnviroment;
            this.logger = logger;
            this.protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
        }
        [AllowAnonymous]  
        
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee().Select( employee =>
            {
                employee.EncryptedId = protector.Protect(employee.Id.ToString());
                return employee;
            });
            return View("~/Views/Home/Index.cshtml",model);
        }

        [AllowAnonymous]
        public ViewResult Show(string id)
        {
            /*throw new Exception("Error in Details view");*/

            int decryptedId = Convert.ToInt32(protector.Unprotect(id));
            Employee employee = _employeeRepository.GetEmployee(decryptedId);
            if(employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound");
            }
            HomeShowViewModel viewModel = new()
            {
                employee = employee,
                pageTitle = "Employee Managment"
        };
            return View(viewModel);
        }

        [HttpGet]
        
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string filename = ProcessUploadedFile(model);
                Employee newEmployee = new Employee()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath =  filename
                };
                _employeeRepository.Add(newEmployee);
                return RedirectToAction("Show", new {id = newEmployee.Id});
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        
        public ViewResult Edit(int id)
        {

            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel viewModel = new EmployeeEditViewModel()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                existingPhotopath = employee.PhotoPath
            };
            return View(viewModel);
        }
        [HttpPost]

        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if (model.Photo != null)
                {
                    if(model.existingPhotopath != null) {
                        string path = Path.Combine(hostingEnviroment.WebRootPath, "images", model.existingPhotopath);
                        System.IO.File.Delete(path);
                    }
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }
            else
            {
                return View();
            }
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            var uploadPath = Path.Combine(hostingEnviroment.WebRootPath, "Images");
            var filename = Guid.NewGuid() + "_" + model.Photo.FileName;
            var filePath = Path.Combine(uploadPath, filename);
            using(var filestream = new FileStream(filePath, FileMode.Create))
            {
                model.Photo.CopyTo(filestream);
            }
            
            return filename;
        }
    }
}
