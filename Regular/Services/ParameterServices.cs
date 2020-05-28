﻿using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Services
{
    public static class ParameterServices
    {
        public static void CreateProjectParameter(Document doc, string parameterName, ParameterType parameterType, string categoryName, BuiltInParameterGroup builtInParameterGroup, bool isInstanceParameter)
        {
            // Spiderinnet's hacky method to create a project parameter, despite the Revit API's limitations on this
            // From https:// spiderinnet.typepad.com/blog/2011/05/parameter-of-revit-api-31-create-project-parameter.html
            // This creates a temporary shared parameters file, a temporary shared parameter
            // It then binds this back to the model as an InstanceBinding and deletes the temporary stuff

            //Creating the necessary categoryset to create the outputParameter
            Category category = CategoryServices.GetCategoryByName(categoryName, doc);
            List<Category> categoriesList = new List<Category>() { category };
            CategorySet categorySet = CategoryServices.GetCategorySetFromList(doc, categoriesList);
            
            using (Transaction transaction = new Transaction(doc, $"Regular - Creating New Project Parameter {parameterName}")) 
            {
                transaction.Start();

                Application revitApplication = RegularApp.RevitApplication;

                string oriFile = revitApplication.SharedParametersFilename;
                string tempFile = Path.GetTempFileName() + ".txt";
                using (File.Create(tempFile)) { }
                revitApplication.SharedParametersFilename = tempFile;
                ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(parameterName, parameterType);
                ExternalDefinition def = revitApplication.OpenSharedParameterFile().Groups.Create("TemporaryDefintionGroup").Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;

                revitApplication.SharedParametersFilename = oriFile;
                File.Delete(tempFile);

                Binding binding = revitApplication.Create.NewTypeBinding(categorySet);
                if (isInstanceParameter) binding = revitApplication.Create.NewInstanceBinding(categorySet);

                BindingMap bindingMap = (new UIApplication(revitApplication)).ActiveUIDocument.Document.ParameterBindings;
                bindingMap.Insert(def, binding, builtInParameterGroup);

                transaction.Commit();
            }
        }
        public static Parameter GetProjectParameterByName(Document doc, string parameterName)
        {
            BindingMap map = doc.ParameterBindings;
            DefinitionBindingMapIterator it = map.ForwardIterator();
            it.Reset();
            while (it.MoveNext())
            {
                string currentParameterName = it.Key.Name;
                if (currentParameterName == parameterName) { return (Parameter)it.Current; }
            }
            return null;
        }
    }
}