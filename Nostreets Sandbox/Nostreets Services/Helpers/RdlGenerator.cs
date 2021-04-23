using Nostreets.Extensions.Helpers.Data;

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace Nostreets_Services.Helpers
{
    public class RdlGenerator : SqlService
    {
        private string _commandText;
        private SqlConnection _connection;
        private string _connectString;
        private ArrayList _fields;

        public XmlElement AddElement(XmlElement parent, string name, string value)
        {
            XmlElement newelement = parent.OwnerDocument.CreateElement(name,
                "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition");
            parent.AppendChild(newelement);
            if (value != null) newelement.InnerText = value;
            return newelement;
        }

        public void GenerateDocument()
        {
            // Create an XML document
            XmlDocument doc = new XmlDocument();

            string xmlData = "<Report " +
            "xmlns=\"http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition\">" +
                "</Report>";

            doc.Load(new StringReader(xmlData));

            #region Report element

            XmlElement report = (XmlElement)doc.FirstChild;
            AddElement(report, "AutoRefresh", "0");
            AddElement(report, "ConsumeContainerWhitespace", "true");

            #endregion Report element

            //DataSources element
            XmlElement dataSources = AddElement(report, "DataSources", null);

            #region DataSource element

            XmlElement dataSource = AddElement(dataSources, "DataSource", null);
            XmlAttribute attr = dataSource.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "DataSource1";

            XmlElement connectionProperties = AddElement(dataSource, "ConnectionProperties", null);
            AddElement(connectionProperties, "DataProvider", "SQL");
            AddElement(connectionProperties, "ConnectString", _connectString);
            AddElement(connectionProperties, "IntegratedSecurity", "true");

            //DataSets element
            XmlElement dataSets = AddElement(report, "DataSets", null);
            XmlElement dataSet = AddElement(dataSets, "DataSet", null);
            attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "DataSet1";

            //Query element
            XmlElement query = AddElement(dataSet, "Query", null);
            AddElement(query, "DataSourceName", "DataSource1");
            AddElement(query, "CommandText", _commandText);
            AddElement(query, "Timeout", "30");

            //Fields element
            XmlElement fields = AddElement(dataSet, "Fields", null);
            XmlElement field = AddElement(fields, "Field", null);

            attr = field.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "CountryName";

            AddElement(field, "DataField", "CountryName");
            field = AddElement(fields, "Field", null);
            attr = field.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "StateProvince";
            AddElement(field, "DataField", "StateProvince");

            #endregion DataSource element

            //end of DataSources

            //ReportSections element
            XmlElement reportSections = AddElement(report, "ReportSections", null);
            XmlElement reportSection = AddElement(reportSections, "ReportSection", null);
            AddElement(reportSection, "Width", "6in");
            AddElement(reportSection, "Page", null);
            XmlElement body = AddElement(reportSection, "Body", null);
            AddElement(body, "Height", "1.5in");
            XmlElement reportItems = AddElement(body, "ReportItems", null);

            // Tablix element
            XmlElement tablix = AddElement(reportItems, "Tablix", null);
            attr = tablix.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "Tablix1";

            AddElement(tablix, "DataSetName", "DataSet1");
            AddElement(tablix, "Top", "0.5in");
            AddElement(tablix, "Left", "0.5in");
            AddElement(tablix, "Height", "0.5in");
            AddElement(tablix, "Width", "3in");

            XmlElement tablixBody = AddElement(tablix, "TablixBody", null);

            //TablixColumns element
            XmlElement tablixColumns = AddElement(tablixBody, "TablixColumns", null);
            XmlElement tablixColumn = AddElement(tablixColumns, "TablixColumn", null);
            AddElement(tablixColumn, "Width", "1.5in");
            tablixColumn = AddElement(tablixColumns, "TablixColumn", null);
            AddElement(tablixColumn, "Width", "1.5in");

            //TablixRows element
            XmlElement tablixRows = AddElement(tablixBody, "TablixRows", null);

            //TablixRow element (header row)
            XmlElement tablixRow = AddElement(tablixRows, "TablixRow", null);
            AddElement(tablixRow, "Height", "0.5in");
            XmlElement tablixCells = AddElement(tablixRow, "TablixCells", null);

            // TablixCell element (first cell)
            XmlElement tablixCell = AddElement(tablixCells, "TablixCell", null);
            XmlElement cellContents = AddElement(tablixCell, "CellContents", null);
            XmlElement textbox = AddElement(cellContents, "Textbox", null);

            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "HeaderCountryName";
            AddElement(textbox, "KeepTogether", "true");

            XmlElement paragraphs = AddElement(textbox, "Paragraphs", null);
            XmlElement paragraph = AddElement(paragraphs, "Paragraph", null);
            XmlElement textRuns = AddElement(paragraph, "TextRuns", null);
            XmlElement textRun = AddElement(textRuns, "TextRun", null);

            AddElement(textRun, "Value", "CountryName");
            XmlElement style = AddElement(textRun, "Style", null);
            AddElement(style, "TextDecoration", "Underline");

            // TablixCell element (second cell)
            tablixCell = AddElement(tablixCells, "TablixCell", null);
            cellContents = AddElement(tablixCell, "CellContents", null);
            textbox = AddElement(cellContents, "Textbox", null);
            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "HeaderStateProvince";
            AddElement(textbox, "KeepTogether", "true");
            paragraphs = AddElement(textbox, "Paragraphs", null);
            paragraph = AddElement(paragraphs, "Paragraph", null);
            textRuns = AddElement(paragraph, "TextRuns", null);
            textRun = AddElement(textRuns, "TextRun", null);
            AddElement(textRun, "Value", "StateProvince");
            style = AddElement(textRun, "Style", null);
            AddElement(style, "TextDecoration", "Underline");

            //TablixRow element (details row)
            tablixRow = AddElement(tablixRows, "TablixRow", null);
            AddElement(tablixRow, "Height", "0.5in");
            tablixCells = AddElement(tablixRow, "TablixCells", null);

            // TablixCell element (first cell)
            tablixCell = AddElement(tablixCells, "TablixCell", null);
            cellContents = AddElement(tablixCell, "CellContents", null);
            textbox = AddElement(cellContents, "Textbox", null);
            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "CountryName";
            AddElement(textbox, "HideDuplicates", "DataSet1");
            AddElement(textbox, "KeepTogether", "true");
            paragraphs = AddElement(textbox, "Paragraphs", null);
            paragraph = AddElement(paragraphs, "Paragraph", null);
            textRuns = AddElement(paragraph, "TextRuns", null);
            textRun = AddElement(textRuns, "TextRun", null);
            AddElement(textRun, "Value", "=Fields!CountryName.Value");
            style = AddElement(textRun, "Style", null);

            // TablixCell element (second cell)
            tablixCell = AddElement(tablixCells, "TablixCell", null);
            cellContents = AddElement(tablixCell, "CellContents", null);
            textbox = AddElement(cellContents, "Textbox", null);
            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "StateProvince";
            AddElement(textbox, "HideDuplicates", "DataSet1");
            AddElement(textbox, "KeepTogether", "true");
            paragraphs = AddElement(textbox, "Paragraphs", null);
            paragraph = AddElement(paragraphs, "Paragraph", null);
            textRuns = AddElement(paragraph, "TextRuns", null);
            textRun = AddElement(textRuns, "TextRun", null);
            AddElement(textRun, "Value", "=Fields!StateProvince.Value");
            style = AddElement(textRun, "Style", null);
            //End of second row

            //End of TablixBody

            //TablixColumnHierarchy element
            XmlElement tablixColumnHierarchy = AddElement(tablix, "TablixColumnHierarchy", null);
            XmlElement tablixMembers = AddElement(tablixColumnHierarchy, "TablixMembers", null);
            AddElement(tablixMembers, "TablixMember", null);
            AddElement(tablixMembers, "TablixMember", null);

            //TablixRowHierarchy element
            XmlElement tablixRowHierarchy = AddElement(tablix, "TablixRowHierarchy", null);
            tablixMembers = AddElement(tablixRowHierarchy, "TablixMembers", null);
            XmlElement tablixMember = AddElement(tablixMembers, "TablixMember", null);
            AddElement(tablixMember, "KeepWithGroup", "After");
            AddElement(tablixMember, "KeepTogether", "true");
            tablixMember = AddElement(tablixMembers, "TablixMember", null);
            AddElement(tablixMember, "DataElementName", "Detail_Collection");
            AddElement(tablixMember, "DataElementOutput", "Output");
            AddElement(tablixMember, "KeepTogether", "true");
            XmlElement group = AddElement(tablixMember, "Group", null);
            attr = group.Attributes.Append(doc.CreateAttribute("Name"));
            attr.Value = "Table1_Details_Group";
            AddElement(group, "DataElementName", "Detail");
            XmlElement tablixMembersNested = AddElement(tablixMember, "TablixMembers", null);
            AddElement(tablixMembersNested, "TablixMember", null);

            //End of Tablix, ReportItems, ReportSections

            //Save XML document to file
            doc.Save("Report1.rdl");
        }

        public void GenerateFieldsList()
        {
            _commandText =
               "SELECT Person.CountryRegion.Name AS CountryName, Person.StateProvince.Name AS StateProvince " +
               "FROM Person.StateProvince " +
               "INNER JOIN Person.CountryRegion ON Person.StateProvince.CountryRegionCode = Person.CountryRegion.CountryRegionCode " +
               "ORDER BY Person.CountryRegion.Name";

            Query.ExecuteCmd(() => Connection, "", null,
                (reader, set) =>
                {
                    for (int i = 0; i <= reader.FieldCount - 1; i++)
                    {
                        if (_fields == null) { _fields = new ArrayList(); }
                        _fields.Add(reader.GetName(i));
                    }
                }, null, null, CommandBehavior.SchemaOnly);
        }

        public void GenerateReport()
        {
            try
            {
                GenerateFieldsList();
                GenerateDocument();

                Console.WriteLine("RDL file generated successfully.");
            }
            catch (Exception exception)
            {
                Console.WriteLine("An error occurred: " + exception.Message);
            }
            finally
            {
                // Close the connection string
                _connection.Close();
            }
        }
    }
}