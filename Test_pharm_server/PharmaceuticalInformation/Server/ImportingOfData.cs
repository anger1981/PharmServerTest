using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using PharmaceuticalInformation.BaseTypes;
using Test_pharm_server;
using System.Linq;
using EntityFramework.Extensions;
using Test_pharm_server.PharmaceuticalInformation.DataTools;

namespace PharmaceuticalInformation.Server
{
    public class ImportingOfData : BaseType
    {

        #region ' Fields '

        //
        private SqlConnection ConnectionToBase;
        private SqlDataAdapter _UpdatingOfData;
        private PhrmInfTESTEntities PhrmInf;
        //
        private Updating.UpdatingOfDataOfInformationForMsSQL UpdatingOfData;

        public class PriceListDrugstore
        {
            public int ID_PR;
            public decimal Price;
            public bool Deleting;
            public bool Preferential;
        }

        #endregion


        #region ' Designer '

        public ImportingOfData(string StringOfConnection)
            : this(StringOfConnection, "")
        {
            //
        }

        public ImportingOfData(string StringOfConnection, string PathToLogFile)
            : base(PathToLogFile)
        {
            //
            // Initializing Fields
            //
            //this.StringOfConnection = StringOfConnection;
            //
            PhrmInf = new PhrmInfTESTEntities(StringOfConnection);

            _UpdatingOfData = new SqlDataAdapter();
            _UpdatingOfData.ContinueUpdateOnError = true;
            //
            // Creating Of Connection
            //
            try
            {
                ConnectionToBase = new SqlConnection(StringOfConnection);
                ConnectionToBase.Open();
                ConnectionToBase.Close();
            }
            catch (Exception E) { throw new Exception(String.Format("Ошибка при создании подключения экспорта: {0}", E)); }
            //
            // !!!
            //
            UpdatingOfData = new Updating.UpdatingOfDataOfInformationForMsSQL(StringOfConnection, PathToLogFile);
        }

        #endregion


        #region ' Creating '

        // Creating Command
        private DbCommand CreatingCommand(string TextOfCommand, DbParameter[] ParametersOfCommand)
        {
            //
            DbCommand CreatedCommand = new SqlCommand(TextOfCommand, ConnectionToBase);
            //
            for (int i = 0; i <= ParametersOfCommand.GetUpperBound(0); i++)
                CreatedCommand.Parameters.Add(ParametersOfCommand[i]);
            // Return
            return CreatedCommand;
        }

        #endregion


        #region ' Getting List Of Private Importers '

        // Getting List Of Private Importers
        public PrivateImporter[] GettingListOfPrivateImporters()
        {
            //
            // Getting Tables Of Private Importers
            //
            DataSet TablesOfPrivateImporters = new DataSet("TablesOfPrivateImporters");

            DataTable DT_PrivImp = new DataTable();
            DataTable DT_RecID = new DataTable();

            try
            {
                PhrmInf.PrivateImportings.AsEnumerable().Fill(ref DT_PrivImp);
                PhrmInf.RecodingIDsOfDrugstoresOfImportings.AsEnumerable().Fill(ref DT_RecID);

                TablesOfPrivateImporters.Tables.Add(DT_PrivImp);
                TablesOfPrivateImporters.Tables.Add(DT_RecID);
            }
            catch (Exception E)
            {
                //
                RecordingInLogFile(String.Format("ERROR Error Of Getting List Private Importers: {0}", E.Message));
                //
                if (TablesOfPrivateImporters == null)
                    TablesOfPrivateImporters = new DataSet("TablesOfPrivateImporters");
            }
            //
            // Creating List Of Private Importers
            //
            PrivateImporter[] ListOfPrivateImporters = new PrivateImporter[0];
            //
            if (TablesOfPrivateImporters.Tables.Count >= 2)
            {
                try
                {
                    //
                    // Processing Tables
                    //
                    TablesOfPrivateImporters.Tables[0].TableName = "PrivateImportings";
                    TablesOfPrivateImporters.Tables[1].TableName = "RecodingIDsOfDrugstoresOfImportings";
                    TablesOfPrivateImporters.Relations.Add(
                        "GettingRecodingIDs", 
                        TablesOfPrivateImporters.Tables["PrivateImportings"].Columns["ID"], 
                        TablesOfPrivateImporters.Tables["RecodingIDsOfDrugstoresOfImportings"].Columns["IDOfPrivateImportings"]);
                    //
                    // Scaning Tables
                    //
                    ListOfPrivateImporters = 
                        new PrivateImporter[TablesOfPrivateImporters.Tables["PrivateImportings"].Rows.Count];
                    int IndexOfImporter = -1;
                    //
                    foreach (DataRow CurrentImporter in TablesOfPrivateImporters.Tables["PrivateImportings"].Rows)
                    {
                        //
                        PrivateImporter.RecodingID[] RecodingIDs = 
                            new PrivateImporter.RecodingID[CurrentImporter.GetChildRows("GettingRecodingIDs").Length];
                        int IndexOfRecoding = -1;
                        foreach (DataRow CurrentRecoding in CurrentImporter.GetChildRows("GettingRecodingIDs"))
                            RecodingIDs[++IndexOfRecoding] = 
                                new PrivateImporter.RecodingID(
                                    (int)CurrentRecoding["IDOfImporter"],
                                    (int)CurrentRecoding["IDOfSystem"]);
                        //
                        ListOfPrivateImporters[++IndexOfImporter] = 
                            new PrivateImporter(
                                CurrentImporter["ID"].ToString(), 
                                CurrentImporter["NameOfImporter"].ToString(), 
                                (bool)CurrentImporter["Active"], 
                                CurrentImporter["PathOfImporting"].ToString(), 
                                (bool)CurrentImporter["UseOfSystemLogin"], 
                                CurrentImporter["MaskOfFileOfImporting"].ToString(), 
                                (bool)CurrentImporter["UseOfRecoding"], 
                                RecodingIDs);
                    }
                }
                catch (Exception E)
                { RecordingInLogFile(String.Format("ERROR Error Of Creating List Private Importings: {0}", E.Message)); }
            }
            //
            // Return
            //
            return ListOfPrivateImporters;
        }

        public class PrivateImporter
        {

            #region ' Fields '

            private string FieldOfID;
            private string FieldOfNameOfImporter;
            private bool FieldOfActive;
            private string FieldOfPathOfImporting;
            private bool FieldOfUseOfSystemLogin;
            private string FieldOfMaskOfFileOfImporting;
            private bool FieldOfUseOfRecoding;
            private RecodingID[] FieldOfRecodingIDs;

            #endregion

            #region ' Designer '

            public PrivateImporter(
                string ID, 
                string NameOfImporter, 
                bool Active, 
                string PathOfImporting,  
                bool UseOfSystemLogin, 
                string MaskOfFileOfImporting, 
                bool UseOfRecoding, 
                RecodingID[] RecodingIDs)
            {
                //
                // Initializing Fields
                //
                this.FieldOfID = ID;
                this.FieldOfNameOfImporter = NameOfImporter;
                this.FieldOfActive = Active;
                this.FieldOfPathOfImporting = PathOfImporting;
                this.FieldOfUseOfSystemLogin = UseOfSystemLogin;
                this.FieldOfMaskOfFileOfImporting = MaskOfFileOfImporting;
                this.FieldOfUseOfRecoding = UseOfRecoding;
                this.FieldOfRecodingIDs = RecodingIDs;
            }

            #endregion

            #region ' Parameters '

            // ID
            public string ID
            {
                get { return FieldOfID; }
            }

            // Active
            public bool Active
            {
                get { return FieldOfActive; }
            }

            // Name Of Importer
            public string NameOfImporter
            {
                get { return FieldOfNameOfImporter; }
            }

            // Path Of Importing
            public string PathOfImporting
            {
                get { return FieldOfPathOfImporting; }
            }

            // Use Of System Login
            public bool UseOfSystemLogin
            {
                get { return FieldOfUseOfSystemLogin; }
            }

            // Mask Of File Of Importing
            public string MaskOfFileOfImporting
            {
                get { return FieldOfMaskOfFileOfImporting; }
            }

            // Use Of Recoding
            public bool UseOfRecoding
            {
                get { return FieldOfUseOfRecoding; }
            }

            // Existence Of Recoding IDs
            public bool ExistenceOfRecodingIDs
            {
                get
                {
                    //
                    bool Result = false;
                    if (FieldOfRecodingIDs != null)
                        if (FieldOfRecodingIDs.Length > 0) Result = true;
                    //
                    return Result; 
                }
            }

            // Recoding IDs
            public RecodingID[] GettingRecodingIDs
            {
                get
                {
                    //
                    RecodingID[] ReturnedRecodingIDs = new RecodingID[FieldOfRecodingIDs.Length];
                    for (int i = 0; i < FieldOfRecodingIDs.Length; i++)
                        ReturnedRecodingIDs[i] = FieldOfRecodingIDs[i];
                    //
                    return FieldOfRecodingIDs;
                }
            }

            #endregion

            public struct RecodingID
            {

                #region ' Fields '

                public int IDOfImporter;
                public int IDOfSystem;

                #endregion

                #region ' Designer '

                public RecodingID(int IDOfImporter, int IDOfSystem)
                {
                    //
                    this.IDOfImporter = IDOfImporter;
                    this.IDOfSystem = IDOfSystem;
                }
                
                #endregion

            }

        }

        #endregion


        #region ' Importing Data '


        #region ' Importing Data From Drugstore '

        // Importing Data From Drugstore
        public void ImportingDataFromDrugstore(DataSet ImportedData)
        {
            //
            // !!!
            //
            this.RecordingInLogFile("Starting Importing Data");
            //
            if (ImportedData != null)
                if (ImportedData.DataSetName == "SendingData")
                {
                    //
                    // Getting ID Of Sending Of Drugstore
                    //
                    int IDOfDrugstore = 0;
                    //
                    try
                    { IDOfDrugstore = (int)ImportedData.Tables["Information"].Rows.Find("IDOfDrugstore")["Value"]; }
                    catch (Exception E)
                    { this.RecordingInLogFile(String.Format("Ошибка при получении ID Аптеки: {0}", E.Message)); }
                    //
                    // !!!
                    //
                    if (IDOfDrugstore > 0)
                    {
                        //
                        // Recording Of Reception
                        //
                        int IDOfReception = RecordingOfReception(
                            (int)ImportedData.Tables["Information"].Rows.Find("IDOfDrugstore")["Value"],
                            ImportedData.Tables.Contains("PriceList"),
                            ImportedData.Tables.Contains("AnnouncementsOfDrugstore"),
                            (DateTime)ImportedData.Tables["Information"].Rows.Find("DateOfSending")["Value"]);
                        //
                        // Recording In Log File
                        //
                        this.RecordingInLogFile(String.Format("ID = {0}", IDOfDrugstore));
                        //
                        if (IDOfReception > 0)
                        {
                            //
                            // Importing Service Data
                            //
                            this.RecordingInLogFile("Importing Service Data");
                            //this.RecordingInLogFile("Starting Importing Service Data");
                            //
                            foreach (DataTable CurrentTable in ImportedData.Tables)
                                switch (CurrentTable.TableName)
                                {
                                    case "Information":
                                        { }
                                        break;
                                    case "InformationOfSettings":
                                        UpdatingInformationOfSettings(CurrentTable, IDOfDrugstore);
                                        break;
                                    case "ListOfSettings":
                                        UpdatingListOfSettings(CurrentTable, IDOfDrugstore);
                                        break;
                                    case "RegistrationOfDrugstores":
                                        UpdatingRegistrationOfDrugstores(CurrentTable, IDOfDrugstore);
                                        break;
                                    case "DatesOfTransfer":
                                        UpdatingDatesOfTransfer(CurrentTable, IDOfDrugstore);
                                        break;
                                    case "LogOfDrugstore":
                                        UpdatingLogOfDrugstore(CurrentTable, IDOfDrugstore);
                                        break;
                                }
                            //
                            //this.RecordingInLogFile("Stoping Importing Service Data");
                            //
                            // Importing Of PriceLists
                            //
                            if (DrugstoreIsActive(IDOfDrugstore))
                            {
                                //
                                if (ImportedData.Tables.Contains("AnnouncementsOfDrugstore") ||
                                    ImportedData.Tables.Contains("PriceList"))
                                {
                                    //
                                    this.RecordingInLogFile("Starting Importing Data Of Drugstore");
                                    //
                                    foreach (DataTable CurrentTable in ImportedData.Tables)
                                        switch (CurrentTable.TableName)
                                        {
                                            case "AnnouncementsOfDrugstore":
                                                UpdatingAnnouncementsOfDrugstore(CurrentTable, IDOfDrugstore);
                                                break;
                                            case "PriceList":
                                                ImportingOfPriceList(CurrentTable, IDOfReception);
                                                break;
                                        }
                                    //
                                    this.RecordingInLogFile("Stoping Importing Data Of Drugstore");
                                }
                            }
                            //
                            // Checking Unknown Tables
                            //
                            this.RecordingInLogFile("Checking Unknown Tables");
                            //
                            foreach (DataTable CurrentTable in ImportedData.Tables)
                            {
                                if ((CurrentTable.TableName != "Information") &&
                                    (CurrentTable.TableName != "InformationOfSettings") &&
                                    (CurrentTable.TableName != "ListOfSettings") &&
                                    (CurrentTable.TableName != "RegistrationOfDrugstores") &&
                                    (CurrentTable.TableName != "DatesOfTransfer") &&
                                    (CurrentTable.TableName != "LogOfDrugstore") &&
                                    (CurrentTable.TableName != "AnnouncementsOfDrugstore") &&
                                    (CurrentTable.TableName != "PriceList"))
                                {
                                    //
                                    this.RecordingInLogFile(
                                        String.Format("Неизвестная таблица в наборе данных: {0}",
                                        CurrentTable.TableName));
                                    //
                                }
                            }
                        }
                        else
                            RecordingInLogFile(String.Format("Wrong IDOfReception: {0}", IDOfDrugstore));
                    }
                    else
                        RecordingInLogFile(String.Format("Wrong IDOfDrugstore: {0}", IDOfDrugstore));
                }
                else
                    RecordingInLogFile("DataSet Name Not equally SendingData");
            else
                this.RecordingInLogFile("DataSet Is Null");
            //
            this.RecordingInLogFile("Stoping Importing Data");
            //this.RecordingInLogFile("");
            //
        }

        // Importing Of Data Of PriceList (Importing Only PriceList)
        public void ImportingOfDataOfPriceList(DataTable PriceList, int IDOfDrugstore)
        {
            //
            // !!!
            //
            if (PriceList != null)
            {
                //
                // Importing Of PriceLists
                //
                if (DrugstoreIsActive(IDOfDrugstore))
                {
                    //
                    // Recording Of Reception
                    //
                    int IDOfReception = RecordingOfReception(
                        IDOfDrugstore, (PriceList.Rows.Count > 0) ? true : false, false, DateTime.Now);
                    //
                    // Importing Of Prices
                    //
                    if (IDOfReception > 0)
                        ImportingOfPriceList(PriceList, IDOfReception);
                }
            }
        }


        #region ' Importing Data In Tables Of Service  ' 

        // Updating Information Of Settings(Version Client)
        private void UpdatingInformationOfSettings(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        PhrmInf.UpdatingInformationOfSettings(IDOfDrugstore, row["key"].ToString(), row["value"].ToString());
                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении регистрационных данных аптеки {0}", IDOfDrugstore));
                    }
                }
                PhrmInf.SaveChanges();
            }            
        }

        // Updating List Of Settings
        private void UpdatingListOfSettings(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        PhrmInf.UpdatingListOfSettings(IDOfDrugstore, row["key"].ToString(), row["value"].ToString());
                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении данных настройки аптеки {0}", IDOfDrugstore));
                    }
                }
                PhrmInf.SaveChanges();
            }
        }

        // Updating Registration Of Drugstores
        private void UpdatingRegistrationOfDrugstores(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        PhrmInf.UpdatingRegistrationOfDrugstores(IDOfDrugstore, Convert.ToInt32(row["ID"]), row["PathToFolderOfPriceLists"].ToString(),
                            row["MaskOfFullPriceList"].ToString(), row["MaskOfIncomingPriceList"].ToString(), row["MaskOfSoldPriceList"].ToString(),
                            Convert.ToBoolean(row["UseOfIDOfPriceList"]), Convert.ToBoolean(row["NotDeletingPriceList"]));
                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении регистрационных данных аптеки {0}", IDOfDrugstore));
                    }
                }
                PhrmInf.SaveChanges();
            }
        }

        // Updating Dates Of Transfer
        private void UpdatingDatesOfTransfer(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        PhrmInf.UpdatingDatesOfTransfer(IDOfDrugstore, Convert.ToInt32(row["ID"]), row["Name"].ToString(), Convert.ToInt32(row["Value"]), Convert.ToDateTime(row["Date"]));

                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении информации об обновлениях справочных данных аптеки {0}", IDOfDrugstore));
                    }
                }
                PhrmInf.SaveChanges();
            }
        }

        // Updating Log Of Drugstore
        private void UpdatingLogOfDrugstore(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        PhrmInf.UpdatingLogsOfDrugstores(IDOfDrugstore, row["Value"].ToString());

                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении данных лога аптеки {0}", IDOfDrugstore));
                    }
                }
                PhrmInf.SaveChanges();
            }
        }

        // Updating Announcements Of Drugstore
        private void UpdatingAnnouncementsOfDrugstore(DataTable TableForUpdating, int IDOfDrugstore)
        {
            if (IDOfDrugstore > 0)
            {
                ////
                //// Update registration data of drugstore
                ////         
                foreach (DataRow row in TableForUpdating.Rows)
                {
                    try
                    {
                        PhrmInf.UpdatingAnnouncementsOfDrugstore(IDOfDrugstore, Convert.ToInt32(row["ID"]), row["Caption"].ToString(), row["Text"].ToString(), Convert.ToBoolean(row["Published"]));

                    }
                    catch
                    {
                        this.RecordingInLogFile(String.Format("Ошибка при обновлении таблицы объявлений, полученных аптекой {0}", IDOfDrugstore));
                    }
                }
                PhrmInf.SaveChanges();
            }            
        }

        // Updating Table Of Service
        private void UpdatingTableOfService(
            DataTable TableForUpdating, string TextOfCommand, string NameOfTable, DbParameter[] ParametersOfCommand)
        {
            //
            // Status Of Modified
            //
            TableForUpdating.AcceptChanges();
            foreach (DataRow CurrentRow in TableForUpdating.Rows)
                CurrentRow.SetModified();
            /*if (NameOfTable != "LogsOfDrugstores")
                CurrentRow.SetModified();
            else
                CurrentRow.SetAdded();*/
            //
            //if (NameOfTable != "LogsOfDrugstores")
            _UpdatingOfData.UpdateCommand = (SqlCommand)CreatingCommand(TextOfCommand, ParametersOfCommand);
            /*else
                _UpdatingOfData.InsertCommand = (SqlCommand)CreatingCommand(TextOfCommand, ParametersOfCommand);*/
            //
            //if (NameOfTable != "LogsOfDrugstores")
            _UpdatingOfData.UpdateCommand.CommandType = CommandType.StoredProcedure;
            //
            // Updating
            //
            UpdateOfUpdatingData(TableForUpdating, NameOfTable);
            //
        }

        #endregion


        #region ' Import Of PriceList '

        // Import Of PriceList
        private void ImportingOfPriceList(DataTable PriceList, int IDOfReception)
        {
            //
            // Converting UInt32 In Int32
            //
            PriceList.Columns.Add("ID_PH2", typeof(Int32));
            PriceList.Columns.Add("ID_PR2", typeof(Int32));
            //
            foreach (DataRow CurrentRow in PriceList.Rows)
            {
                //
                try
                {
                    CurrentRow["ID_PH2"] = Convert.ToInt32(CurrentRow["ID_PH"]);
                    CurrentRow["ID_PR2"] = Convert.ToInt32(CurrentRow["ID_PR"]);
                }
                catch { this.RecordingInLogFile("Ошибка при конвертации ID_PH and ID_PR"); }
            }
            //
            try
            {
                if (PriceList.PrimaryKey.Length != 0)
                    PriceList.PrimaryKey = new DataColumn[2] { PriceList.Columns["ID_PH2"], PriceList.Columns["ID_PR2"] };
            }
            catch { this.RecordingInLogFile("asl"); }
            //
            PriceList.Columns.Remove("ID_PH");
            PriceList.Columns.Remove("ID_PR");
            //
            PriceList.Columns["ID_PH2"].ColumnName = "ID_PH";
            PriceList.Columns["ID_PR2"].ColumnName = "ID_PR";
            //
            // Converting Null And DBNUll in 0
            //
            foreach (DataRow CurrentPrice in PriceList.Rows)
                if ((CurrentPrice["Price"] == null) || (CurrentPrice["Price"] is DBNull))
                    CurrentPrice["Price"] = 0;
            //
            // Getting IDs Of Drugstores
            //
            DataTable TableOfIDsOfDrugstores = new DataView(PriceList).ToTable(true, "ID_PH");
            int[] IDsOfDrugstores = new int[TableOfIDsOfDrugstores.Rows.Count];
            for (int i = 0; i < IDsOfDrugstores.Length; i++)
                IDsOfDrugstores[i] = (int)TableOfIDsOfDrugstores.Rows[i]["ID_PH"];
            //
            // Checking Existence Of Drugstores
            //
            System.Collections.ArrayList CheckedIDs = new System.Collections.ArrayList();
            foreach (int CurrentID in IDsOfDrugstores)
            {
                if (DrugstoreIsActive(CurrentID))
                    CheckedIDs.Add(CurrentID);
            }
            IDsOfDrugstores = new int[0];
            IDsOfDrugstores = (int[]) CheckedIDs.ToArray(typeof(int));
            //
            // Recording In Reports Of Importing
            //
            bool Successful = true;
            foreach (int CurrentID in IDsOfDrugstores)
            { Successful = RecordingInReportsOfImporting(CurrentID, IDOfReception); if (!Successful) break; }
            //
            // Importing Of Drugstores
            //
            //int CountOfImporting = 0;
            DataView GettingDrugstore = new DataView(PriceList);
            for (int i = 0; ((i < IDsOfDrugstores.Length) && Successful); i++)
            {
                //
                // Getting Prices Of Drugstore
                //
                GettingDrugstore.RowFilter = String.Format("ID_PH={0}", IDsOfDrugstores[i]);
                DataTable PricesOfDrugstore = GettingDrugstore.ToTable("PricesOfDrugstore");
                //
                // Addition Of TMP Of ID Of Prices
                //
                PricesOfDrugstore.Columns.Add(new DataColumn("ID", typeof(int)));
                for (int i2 = 0; i2 < PricesOfDrugstore.Rows.Count; i2++)
                    PricesOfDrugstore.Rows[i2]["ID"] = (i2 + 1);
                PricesOfDrugstore.PrimaryKey = new DataColumn[1] { PricesOfDrugstore.Columns["ID"] };
                //
                // Filling ID Of Prices And ID_PR Of Products
                //
                int[,] IDAndIDPROfPrices = new int[PricesOfDrugstore.Rows.Count, 2];
                for (int i2 = 0; i2 <= IDAndIDPROfPrices.GetUpperBound(0); i2++)
                {
                    IDAndIDPROfPrices[i2, 0] = (int)PricesOfDrugstore.Rows[i2]["ID"];
                    IDAndIDPROfPrices[i2, 1] = (int)PricesOfDrugstore.Rows[i2]["ID_PR"];
                }
                //
                // Search And Liquidation Of Recurrences
                //
                for (int i2 = 0; i2 <= IDAndIDPROfPrices.GetUpperBound(0); i2++)
                    for (int i3 = 0; i3 <= IDAndIDPROfPrices.GetUpperBound(0); i3++)
                        if ((i2 != i3) && (i2 > i3) && (IDAndIDPROfPrices[i2, 1] == IDAndIDPROfPrices[i3, 1]))
                        {
                            DataRow GetRow = PricesOfDrugstore.Rows.Find(IDAndIDPROfPrices[i3, 0]);
                            if (GetRow != null)
                                GetRow.Delete();
                        }
                //
                PricesOfDrugstore.AcceptChanges();
                //
                // Getting AllPrices From Drugstore
                //
                /*
                 object Obj = PricesOfDrugstore.Rows[i2]["AllPrices"];
                 Console.WriteLine(Obj is bool);
                 Console.WriteLine(Obj == null);
                 Console.WriteLine(Obj is DBNull);
                 */
                bool AllPrices = false;
                for (int i2 = 0; i2 < PricesOfDrugstore.Rows.Count; i2++)
                    if (!(PricesOfDrugstore.Rows[i2]["AllPrices"] is DBNull))
                        if ((bool)PricesOfDrugstore.Rows[i2]["AllPrices"])
                        { AllPrices = true; break; }
                //
                // Recording In Reports Of Importing Of PriceList
                //
                UpdatingReportsOfImporting(
                    IDsOfDrugstores[i], IDOfReception, PricesOfDrugstore.Rows.Count, AllPrices);
                //
                // Clearing Columns Of Drugstore
                //
                DataView FilteringOfColumns = new DataView(PricesOfDrugstore);
                PricesOfDrugstore =
                    FilteringOfColumns.ToTable(
                    "PricesOfDrugstore", true, "ID_PR", "Price", "Deleting", "Preferential");
                PricesOfDrugstore.PrimaryKey = new DataColumn[] { PricesOfDrugstore.Columns["ID_PR"] };
                //
                // Importing Of Prices Of Drugstore
                //
                //CountOfImporting += PricesOfDrugstore.Rows.Count;
                ImportingOfPricesOfDrugstore(IDOfReception, IDsOfDrugstores[i], AllPrices, PricesOfDrugstore);
                //
            }
            // Return
            //return;// CountOfImporting;
        }

        // Importing Of PriceList Of Drugstore
        private void ImportingOfPricesOfDrugstore(int IDOfReception, int IDOfDrugstore, bool AllPrices, DataTable PriceList)
        {
            //
            DataTable PricesOfDrugstore = PriceList.Copy();

            int CountOfModification = 0;
            //
            // Refreshing Of Dates
            //
            if ((AllPrices) && (PricesOfDrugstore.Rows.Count >= 1))// && PricesOfDrugstore.Rows.Count > 100) // !!! Отключение ограничения
            {
                //
                // Creating List Of Id_Product For Updating Deleting. The actual prices of pharmacy products that are not listed in the incoming PriceList should be marked as deleted
                //

                // Take Id_Product actual prices
                DataTable IDsOfProducts = new DataTable();
                PhrmInf.price_list.Where(p => p.Id_Pharmacy == IDOfDrugstore && !p.Is_deleted).Select(p => new { p.Id_Product }).Fill(ref IDsOfProducts);
                
                // Create table for product that should be marked deleted in pricelist
                DataTable IDsForDeleting = new DataTable();
                IDsForDeleting.Columns.Add("ID", typeof(int));
                //
                foreach (DataRow CurrentIDOfProduct in IDsOfProducts.Rows)
                {
                    bool Addition = true;
                    foreach (DataRow CurrentPriceOfDrugstore in PricesOfDrugstore.Rows)
                        if (((int)CurrentIDOfProduct[0]) == ((int)CurrentPriceOfDrugstore["ID_PR"]))
                        { Addition = false; break; }
                    if (Addition)
                        IDsForDeleting.Rows.Add(CurrentIDOfProduct[0]);
                }
                //
                CountOfModification += IDsForDeleting.Rows.Count;
                //
                // Creating Command Of Updating Deleting
                //
                IEnumerable<dynamic> IDsForDeleting_IE = IDsForDeleting.AsEnumerable();

                IEnumerable<int> IDsForDeleting_IE_i = IDsForDeleting_IE.Select(p => (int) p.ID);

                IEnumerable<HistoryOfChangesOfPrice> IDsForDeleting_IE_HP = IDsForDeleting_IE
                    .Select(p => new HistoryOfChangesOfPrice
                    {
                        IDOfDrugstore = IDOfDrugstore,
                        IDOfProduct = (int)p.ID,
                        ModificationOfPrice = 3,
                        ModifiedPrice = 0,
                        DateOfChange = DateTime.Now
                    });

                try
                {
                    PhrmInf.price_list.Where(pl => pl.Id_Pharmacy == IDOfDrugstore && !pl.Is_deleted)
                    .Join(IDsForDeleting_IE_i, p => p.Id_Product, pt => pt, (p, pt) => p)
                    .UpdateAsync(pl => new price_list { Is_deleted = true });

                    PhrmInf.HistoryOfChangesOfPrices.AddRange(IDsForDeleting_IE_HP);
                    //UpdateOfUpdatingData(IDsForDeleting, String.Format("Price_list Deleting {0}", IDOfDrugstore));
                }
                catch (Exception E)
                {
                    //
                    //if (ConnectionToBase.State == ConnectionState.Open)
                    //    ConnectionToBase.Close();
                    //
                    RecordingInLogFile(String.Format("Ошибка при пометке на удаление: {0}", E.Message));
                }
                //
                // Recording In Reports Of Importing Of PriceList
                //
                UpdatingReportsOfImporting(IDOfDrugstore, IDOfReception, IDsForDeleting.Rows.Count);
            }
           
            ////
            //// Updating and Inserting
            ////
            //UpdateOfUpdatingData(PricesOfDrugstore, String.Format("Price_list {0}", IDOfDrugstore));

            IEnumerable<PriceListDrugstore> PricesOfDrugstore_IE = PricesOfDrugstore.AsEnumerable()
                .Select(p => new PriceListDrugstore { ID_PR = p.ID_PR, Price = p.Price, Deleting = p.Deleting, Preferential = p.Preferential }).ToArray();

            IEnumerable<PriceListDrugstore> pld_upd = PricesOfDrugstore_IE.Join(PhrmInf.price_list.Where(pl => pl.Id_Pharmacy == IDOfDrugstore),
                pld => pld.ID_PR, p => p.Id_Product, (pld, p) => pld);

            IEnumerable <PriceListDrugstore> pld_ins =
                    PricesOfDrugstore_IE.Where(pld => !PhrmInf.price_list.Where(p => p.Id_Product == pld.ID_PR && p.Id_Pharmacy == IDOfDrugstore).Any());


            try
            {
                //Update existing in summary price list prices from PriceListDrugstore

                //foreach (PriceListDrugstore pld in pld_upd)
                //    PhrmInf.UpdatingPriceList(IDOfDrugstore, IDOfReception, pld.ID_PR, pld.Price, pld.Deleting, pld.Preferential);

                PricesOfDrugstore_IE.Join(PhrmInf.price_list.Where(pl => pl.Id_Pharmacy == IDOfDrugstore),
                pld => pld.ID_PR, p => p.Id_Product, (pld, p) => pld)
                .Select(pld => PhrmInf.UpdatingPriceList(IDOfDrugstore, IDOfReception, pld.ID_PR, pld.Price, pld.Deleting, pld.Preferential)).ToList();

                //PhrmInf.price_list.Where(pl => pl.Id_Pharmacy == IDOfDrugstore)
                //    .Join(PricesOfDrugstore_IE, p => p.Id_Product, pld => pld.ID_PR
                //          ,(p, pld) => pld)
                //          .Select(pld => PhrmInf.UpdatingPriceList(IDOfDrugstore, IDOfReception, pld.ID_PR, pld.Price, pld.Deleting, pld.Preferential)).ToList();



                //Inserting new price, which not exists in summary price_list

                PricesOfDrugstore_IE.Where(pld => !PhrmInf.price_list.Where(p => p.Id_Product == pld.ID_PR && p.Id_Pharmacy == IDOfDrugstore).Any())
                .Select(pld => PhrmInf.InsertingInPriceList(IDOfDrugstore, IDOfReception, pld.ID_PR, pld.Price, pld.Deleting, pld.Preferential)).ToList();
                //foreach (PriceListDrugstore pld in pld_ins)
                //    PhrmInf.InsertingInPriceList(IDOfDrugstore, IDOfReception, pld.ID_PR, pld.Price, pld.Deleting, pld.Preferential);

                //PhrmInf.InsertingInPriceList(IDOfDrugstore, IDOfReception, 454, 10000, false, false);



            }
            catch (Exception E)
            {
                //
                //if (ConnectionToBase.State == ConnectionState.Open)
                //    ConnectionToBase.Close();
                //
                RecordingInLogFile(String.Format("Ошибка при обновлении существующих и вставке новых позиций из файла остатков: {0}", E.Message));
            }

            PhrmInf.SaveChanges();

            //
            // Updating Date Of Actuals
            //
            if ((CountOfModification > 0) && (AllPrices == true))
            {
                //
                UpdatingDateOfActuals(IDOfDrugstore);
            }
            //
        }

        // Reading Status
        private void ReadingStatus(DataTable TableForReading)
        {
            //
            int S_M = 0, S_A = 0, S_R = 0, S_U = 0;
            for (int i = 0; i < TableForReading.Rows.Count; i++)
                if (TableForReading.Rows[i].RowState == DataRowState.Added)
                    S_A++;
                else if (TableForReading.Rows[i].RowState == DataRowState.Deleted)
                    S_R++;
                else if (TableForReading.Rows[i].RowState == DataRowState.Modified)
                    S_M++;
                else if (TableForReading.Rows[i].RowState == DataRowState.Unchanged)
                    S_U++;
            RecordingInLogFile(
                String.Format("CountR={4} Add={0} Rem={1} Mod={2} Unc={3}",
                S_A, S_R, S_M, S_U, TableForReading.Rows.Count));
        }

        // Recording In Reports Of Importing
        private bool RecordingInReportsOfImporting(int IDOfDrugstore, int IDOfReception)
        {
            //
            bool Successful = true;
            //
            // Creating Command Of Recording
            //

            ReportsOfImportingOfPriceList ripl = new ReportsOfImportingOfPriceList
            {
                ID_PH = IDOfDrugstore,
                ID_HR = IDOfReception,
                CountNotConfirmed = 0,
                CountOfAdditions = 0,
                CountOfUnAdditions = 0,
                CountOfChanges = 0,
                CountOfUnChanges = 0,
                CountOfDeletings = 0,
                CountOfAllPrices = 0
            };

            try
            {
                PhrmInf.ReportsOfImportingOfPriceLists.Add(ripl);
                PhrmInf.SaveChanges();
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при записе в ReportsOfImportingOfPriceLists: {0}", E.Message));
                Successful = false;
            }
            // Return
            return Successful;
        }

        // Updating Reports Of Importing
        private void UpdatingReportsOfImporting(int IDOfDrugstore, int IDOfReception, int CountOfPrices, bool FullPriceList)
        {
            //
            // Creating Command Of Recording
            //
            try
            {
                PhrmInf.ReportsOfImportingOfPriceLists.Where(ripl => ripl.ID_PH == IDOfDrugstore && ripl.ID_HR == IDOfReception).
                   Update(ripl_n => new ReportsOfImportingOfPriceList { CountOfAllPrices = CountOfPrices, FullPriceList = FullPriceList });
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при обновлении ReportsOfImportingOfPriceLists: {0}", E.Message));
            }
        }

        // Updating Reports Of Importing
        private void UpdatingReportsOfImporting(int IDOfDrugstore, int IDOfReception, int CountOfDeleting)
        {
            //
            // Creating Command Of Recording
            //
            try
            {
                PhrmInf.ReportsOfImportingOfPriceLists.Where(ripl => ripl.ID_PH == IDOfDrugstore && ripl.ID_HR == IDOfReception).
                   Update(ripl_n => new ReportsOfImportingOfPriceList { CountNotConfirmed = CountOfDeleting });
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при обновлении ReportsOfImportingOfPriceLists: {0}", E.Message));
            }
        }

        // Updating Date Of Actuals
        private void UpdatingDateOfActuals(int IDOfDrugstore)
        {
            //
            // Creating Command Of Updating
            //            
            try
            {
                PhrmInf.price_list.Where(pl => pl.Id_Pharmacy == IDOfDrugstore && !pl.Is_deleted).
                    Update(plu => new price_list { Actual = DateTime.Now });
            }
            catch (Exception E)
            {
                //
                this.RecordingInLogFile(
                    String.Format("Ошибка при обновлении price_list (Date Of Actuals): {0}", E.Message));
            }
        }

        // TMP
        protected void SetStatusOfRows(DataTable TableForStatus,
            string TextOfCheckingOfDeleting, string TextOfCheckingOfExisting, DbParameter[] ParametersOfCommand)
        {
            //
            try
            {
                //
                TableForStatus.Columns.Add("TMP_Status", typeof(string));
                //
                TableForStatus.AcceptChanges();
                foreach (DataRow CurrentRow in TableForStatus.Rows)
                    CurrentRow.SetModified();
                //
                string TextOfExistingOfCommand = "";
                if (TextOfCheckingOfDeleting != "")
                    TextOfExistingOfCommand = String.Format(
                        "IF ({0}) SET @Status = 'REM' ELSE IF ({1}) SET @Status = 'MOD' ELSE SET @Status = 'ADD';",
                        TextOfCheckingOfDeleting, TextOfCheckingOfExisting);
                else
                    TextOfExistingOfCommand = String.Format(
                        "IF ({0}) SET @Status = 'MOD' ELSE SET @Status = 'ADD';",
                        TextOfCheckingOfExisting);
                //
                DbParameter[] ParametersOfExistingCommand =
                    new DbParameter[ParametersOfCommand.Length + 1];
                for (int i = 0; i < ParametersOfCommand.Length; i++)
                    ParametersOfExistingCommand[i] = ParametersOfCommand[i];
                ParametersOfExistingCommand[ParametersOfCommand.Length] =
                    new SqlParameter("@Status", SqlDbType.VarChar, 3, "TMP_Status");
                //
                DbCommand CommandOfExisting = CreatingCommand(TextOfExistingOfCommand, ParametersOfExistingCommand);
                CommandOfExisting.Parameters["@Status"].Direction = ParameterDirection.Output;
                SqlDataAdapter ReadingStatus = new SqlDataAdapter();
                ReadingStatus.UpdateCommand = (SqlCommand)CommandOfExisting;
                //
                ReadingStatus.Update(TableForStatus);
                //
                foreach (DataRow CurrentRow in TableForStatus.Rows)
                    switch (CurrentRow["TMP_Status"].ToString())
                    {
                        case "REM":
                            CurrentRow.Delete();
                            break;
                        case "ADD":
                            CurrentRow.SetAdded();
                            break;
                        case "MOD":
                            CurrentRow.SetModified();
                            break;
                    }
                //
                TableForStatus.Columns.Remove("TMP_Status");
            }
            catch (Exception E) { ReturningMessageAboutError("Ошибка при чтении статуса строк", E, false); }
        }

        // TMP
        private void UpdateOfUpdatingData(DataTable DataForUpdating, string TableName)
        {
            //
            // Updating
            //
            if ((TableName != "Information") &&
                (TableName != "InformationOfSettings") &&
                (TableName != "ListOfSettings") &&
                (TableName != "RegistrationOfDrugstores") &&
                (TableName != "DatesOfTransfer") &&
                (TableName != "LogsOfDrugstores") &&
                (TableName != "AnnouncementsOfDrugstore") &&
                (TableName != "PriceList"))
                RecordingInLogFile(String.Format("Start Updating Table Of {0}", TableName));
            //
            int CountOfUpdating = 0;
            try { CountOfUpdating = _UpdatingOfData.Update(DataForUpdating); }
            catch (Exception E)
            { ReturningMessageAboutError(String.Format("Ошибка при обновлении таблицы {0}", TableName), E, false); }
            //
            if ((TableName != "Information") &&
                (TableName != "InformationOfSettings") &&
                (TableName != "ListOfSettings") &&
                (TableName != "RegistrationOfDrugstores") &&
                (TableName != "DatesOfTransfer") &&
                (TableName != "LogsOfDrugstores") &&
                (TableName != "AnnouncementsOfDrugstore") &&
                (TableName != "PriceList"))
                RecordingInLogFile(String.Format("End Updating Table Of {0}", TableName));
            //
            // Clearing Of UpdatingOfData 
            //
            _UpdatingOfData.InsertCommand = null;
            _UpdatingOfData.UpdateCommand = null;
            _UpdatingOfData.DeleteCommand = null;
        }

        #endregion


        #region ' Checking '

        // Drugstore Is Active
        public bool DrugstoreIsActive(int IDOfDrugstore)
        {
            bool ResultOfActivation = false;

            try
            {
                int CountOfDrugstore = PhrmInf.Pharmacies.Where(p => !p.Is_deleted && p.Id_Pharmacy == IDOfDrugstore).Count();
                //
                ResultOfActivation = (CountOfDrugstore > 0) ? true : false;
            }
            catch (Exception E)
            {
                RecordingInLogFile(String.Format("ERROR Ошибка при проверке активации аптеки: {0}", E.Message));
            }
            //
            // NO 103
            //
            if (IDOfDrugstore == 103)
                ResultOfActivation = false;
            //
            // Return
            //
            return ResultOfActivation;
        }

        #endregion


        #region ' Management Of Connection '

        // Opening Connection
        private void OpeningConnection(DbConnection Connection)
        {
            //
            if (Connection != null)
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
        }

        // Closing Connection
        private void ClosingConnection(DbConnection Connection)
        {
            //
            if (Connection != null)
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();
        }

        #endregion


        #endregion


        #region ' Importing Data From Service Of Help TMP '

        // Importing Data From Service Of Help
        public void ImportingDataFromServiceOfHelp(DataSet ImportedData)
        {
            if (ImportedData != null)
            {
                //
                // Recording Of Reception
                //
                bool ContainsPriceList = false;
                if (ImportedData.Tables.Contains("PriceList"))
                    if (ImportedData.Tables["PriceList"].Rows.Count > 0)
                        ContainsPriceList = true;
                //
                /*
                this.RecordingInLogFile(
                    String.Format("ImportingDataFromServiceOfHelp02 Count = {0}", ImportedData.Tables.Count));
                */
                //
                int IDOfReception = RecordingOfReception(108, ContainsPriceList, false, DateTime.Now);
                //
                foreach (DataTable CurrentTable in ImportedData.Tables)
                {
                    //
                    switch (CurrentTable.TableName)
                    {
                        case "Pharmacy":
                            {
                                //
                                /*
                                this.RecordingInLogFile(
                                    String.Format("{0} A {1}", CurrentTable.TableName, CurrentTable.Rows.Count));
                                */
                                /*
                                //
                                // Deleting New Rows In Pharmacy
                                //
                                // Обрыв тотальный ???
                                ClearingNewRowsInPharmacy(CurrentTable);
                                //
                                this.RecordingInLogFile(
                                    String.Format("{0} B {1}", CurrentTable.TableName, CurrentTable.Rows.Count));
                                //
                                // Filling Updating Of Date
                                //
                                foreach (DataRow CurrentRow in CurrentTable.Rows)
                                    CurrentRow["Updating"] = DateTime.Now;
                                //
                                // Updating 
                                //
                                this.RecordingInLogFile(
                                    String.Format("{0} C {1}", CurrentTable.TableName, CurrentTable.Rows.Count));
                                //
                                UpdatingOfData.UpdatingOfPharmacy(CurrentTable);
                                */
                            }
                            break;
                        case "Products":
                            {
                                //
                                /*this.RecordingInLogFile(
                                    String.Format("{0} {1}", CurrentTable.TableName, CurrentTable.Rows.Count));*/
                                //
                                DataTable Products = CurrentTable.Copy();
                                //
                                // Filling Updating Of Date
                                //
                                foreach (DataRow CurrentRow in Products.Rows)
                                    CurrentRow["Updating"] = DateTime.Now;
                                //
                                // Renaming Name
                                //
                                foreach (DataRow CurrentProduct in Products.Rows)
                                {
                                    //
                                    if ((CurrentProduct["Name"] != null) &&
                                        !(CurrentProduct["Name"] is DBNull))
                                    {
                                        string NameOfProduct = CurrentProduct["Name"].ToString();
                                        if (NameOfProduct.Length > 2)
                                            if (NameOfProduct.EndsWith("\n") &&
                                                (NameOfProduct[NameOfProduct.Length - 2] != '\n'))
                                                NameOfProduct = NameOfProduct.Remove(NameOfProduct.Length - 2, 2);
                                        //
                                        NameOfProduct = NameOfProduct.Trim();
                                        CurrentProduct["Name"] = NameOfProduct;
                                    }
                                }
                                Products.AcceptChanges();
                                //
                                // Updating 
                                //
                                /*this.RecordingInLogFile(
                                    String.Format("{0} {1}", Products.TableName, Products.Rows.Count));*/
                                //
                                UpdatingOfData.UpdatingOfProducts(Products);
                            }
                            break;
                        case "PriceList":
                            {
                                //
                                /*this.RecordingInLogFile(
                                    String.Format("{0} {1}", CurrentTable.TableName, CurrentTable.Rows.Count));*/
                                //
                                // Renaming Table And Columns
                                //
                                DataTable PriceList = CurrentTable.Copy();
                                //
                                // Addition Of Column AllPrices
                                //
                                PriceList.Columns.Add("AllPrices", typeof(bool));
                                foreach (DataRow CurrentPrice in PriceList.Rows)
                                    CurrentPrice["AllPrices"] = false;
                                //
                                PriceList.AcceptChanges();
                                /*
                                //
                                // Recording Of Reception
                                //
                                int IDOfReception = RecordingOfReception(
                                    108, (PriceList.Rows.Count > 0) ? true : false, false, DateTime.Now);
                                */
                                //
                                // Importing Of Prices
                                //
                                /*this.RecordingInLogFile(
                                    String.Format("{0} {1}", PriceList.TableName, PriceList.Rows.Count));*/
                                //
                                if (IDOfReception > 0)
                                    ImportingOfPriceList(PriceList, IDOfReception);
                                //
                            }
                            break;
                        case "LogOfService":
                            {
                                //IDsOfModifications
                                /*this.RecordingInLogFile(
                                    String.Format("{0} {1}", CurrentTable.TableName, CurrentTable.Rows.Count));*/
                                //
                                //UpdatingLogOfDrugstore(CurrentTable, 108);
                            }
                            break;
                        case "IDsOfModifications":
                            {
                                //IDsOfModifications
                            }
                            break;
                        case "InformationOfData":
                            {
                                //InformationOfData
                            }
                            break;
                        default:
                            {
                                //
                                this.RecordingInLogFile(
                                    String.Format("Неизвестная таблица в наборе данных {0}", CurrentTable.TableName));
                            }
                            break;
                    }
                }
                //
                //this.RecordingInLogFile("");
            }
        }

        // Clearing New Rows In Pharmacy
        public void ClearingNewRowsInPharmacy(DataTable DataForPharmacy)
        {
            //
            // Creating Command Of Reading Of Status Of Rows
            //
            DbParameter[] ParametersOfSelectionCommand = new DbParameter[1] {
                new SqlParameter("@P1", SqlDbType.Int, 0, "Id_Pharmacy") };
            SetStatusOfRows(DataForPharmacy,
                "", "EXISTS(SELECT Id_Pharmacy FROM Pharmacy WHERE Id_Pharmacy = @P1)", ParametersOfSelectionCommand);
            //
            // Clearing Pharmacy
            //
            foreach (DataRow CurrentRow in DataForPharmacy.Rows)
                if (CurrentRow.RowState == DataRowState.Added)
                {
                    CurrentRow.AcceptChanges();
                    CurrentRow.Delete();
                }
            //
            DataForPharmacy.AcceptChanges();
            //
        }

        #endregion


        // Recording Of Reception
        private int RecordingOfReception(
            int IDOfDrugstore, bool ContainsPriceList, bool ContainsAnnouncements, DateTime LocalDateOfSending)
        {
            //
            // Generation New HistoryOfReception
            //
            HistoryOfReception hr = new HistoryOfReception
            {
                ID_PH = IDOfDrugstore,
                ContainsPriceList = ContainsPriceList,
                ContainsAnnouncements = ContainsAnnouncements,
                DateOfReception = DateTime.Now,
                LocalDateOfSending = LocalDateOfSending
            };            
            //
            // Executing
            //
            try
            {
                PhrmInf.HistoryOfReceptions.Add(hr);
                PhrmInf.SaveChanges();
            }
            catch (Exception E)
            {
                this.RecordingInLogFile(
                    String.Format("Ошибка при регистрации приема данных ID {0}: {1}",
                    IDOfDrugstore, E.Message));
            }
            //
            // Getting ID Of Reception Of Data
            //
            int IDOfReception = 0;
            try
            {
                IDOfReception = PhrmInf.HistoryOfReceptions.Max(i => i.ID);
            }
            catch { this.RecordingInLogFile("Ошибка при получении IDOfReception"); }
            //
            // Return
            //
            return IDOfReception;
        }


        #endregion

    }
}