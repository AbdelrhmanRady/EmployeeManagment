﻿namespace WebApplication1.ViewModels
{
    public class EmployeeEditViewModel : EmployeeCreateViewModel
    {
        public int Id { get; set; }
        public string existingPhotopath { get; set; }
    }
}
