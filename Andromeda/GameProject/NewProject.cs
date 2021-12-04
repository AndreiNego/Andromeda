﻿using Andromeda.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Andromeda.Utilities;
using System.Collections.ObjectModel;

namespace Andromeda.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }

        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }

        public string ScreenshotFilePath { get; set; }

        public string IconFilePath { get; set; }

        public string ProjectFilePath { get; set; }
    }
    public class NewProject : ViewModelBase
    {
        private readonly string _templatePath = @"..\..\Andromeda\ProjectTemplates";
        private string _projectName = "NewProject";
        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (_projectName != value)
                {
                    _projectName = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectName));
                }

                
            }
        }

        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\AndromedaProject\";
        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectPath));
                }

               
            }
        }

        private bool _isPathValid;

        public bool isPathValid
        {
            get => _isPathValid;
            set
            {
                if(_isPathValid != value)
                {
                    _isPathValid = value;
                    OnPropertyChanged(nameof(isPathValid));
                }
            }
        }

        private string _errorMsg;
        public string ErrorMsg
        {
            get => _errorMsg;
            set
            {
                if (_errorMsg != value)
                {
                    _errorMsg = value;
                    OnPropertyChanged(nameof(ErrorMsg));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();

        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates
        { get; }

        private bool ValidateProjectPath()
        {
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path))
            {
                path += @"\";
            }
            path += $@"{ProjectName}\";
            isPathValid = true;
            if (string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMsg = "Type in a project name.";
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMsg = "Invalid character(s) used in project name.";
            }
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMsg = "Select a valid project folder";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMsg = "Invalid path.";
            } 
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMsg = "Selected project folder already exists and is not empty.";
            }
            else
            {
                ErrorMsg = string.Empty;
                isPathValid = true;
            }
            return isPathValid;
        }

        public string CreateProject(ProjectTemplate template)
        {
            ValidateProjectPath();
            if (!isPathValid)
            {
                return string.Empty;
            }
           
            if (!Path.EndsInDirectorySeparator(ProjectPath))
            {
                ProjectPath += @"\";
            }
            var path = $@"{ProjectPath}{ProjectName}\";
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                foreach (var folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
                }
                var dirInfo = new DirectoryInfo(path + @".Andromeda\");
                dirInfo.Attributes |= FileAttributes.Hidden;
                //    File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.jpg")));

                var project = new Project(ProjectName, path);
                Serializer.ToFile(project, path + $"{ProjectName}" + Project.Extension);
                return path;
                
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
                return string.Empty;
            }

        }
        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try
            {
                var templatesFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templatesFiles.Any());
                foreach (var file in templatesFiles)
                {
                   var template = Serializer.FromFile<ProjectTemplate>(file);
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.jpg"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                    _projectTemplates.Add(template);
               /*     var template = new ProjectTemplate()
                    {
                        ProjectType = "EmptyProject",
                        ProjectFile = "project.Andromeda",
                        Folders = new List<string>() { ".Andromeda", "Content", "GameCode" }
                    };
                    Serializer.ToFile(template, file);
               */
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //TODO: Log Error
            }
        }
    }
  
}
