using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class InquiryRepository
    {
        private SqlConnection con;
        DataTable d=new DataTable();

        DataTable dgFormulation = new DataTable();
        DataTable dgCompleteCosting = new DataTable();
        DataTable dgComponents = new DataTable();
        CostingResult costing = new CostingResult();

        List<string> ListOfGenCodes = new List<string>();
        List<string> ListOfworkorder = new List<string>();
        double ComponentTotal = 0.0;
        decimal _totalFormulation = 0.0m;
        double TotalFormulationRate_perpack_perD = 0.0;
        double TotalFormulationRate_perpack_perWo = 0.0;
        //string combicode = "";
        string dsgcode = "";
        string drugLicense = "";
        string wono = "";
        string GENCODE = "";
        string gen_code = "";
        string dsg_code = "";
        string desc_code = "";
        string work_order = "";
        string Size = "";
        double ccpoq = 0.0;
        double ccpdq = 0.0;
        string combicode = "";
        string Field11txtSize = "";
        string Field3txtUnitSize = "";
        string Field4txtWeightOfUnit = "";
        string Field10txtPowderWeightPerUnit = "";
        string Field5txtWeightUnit = "";
        string Field9txtMuliplierFactor = "";
        string Field7txtTotNoUnitInBatch = "";
        public string ConnectionStr { get; set; }
        private void Connection()
        {
            con = new SqlConnection(ConnectionStr);
        }
        public InquiryRepository(string connectionString)
        {
            ConnectionStr = connectionString;
        }
        public bool AddInquiryItem(List<Inquiry> model)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStr))
                {
                    connection.Open();
                    int totalRowsAffected = 0;
                    int result = 0;
                    foreach (var item in model)
                    {
                        using (SqlCommand command = new SqlCommand("InsertOrUpdateQuotation", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@item_code", item.item_code ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Party_Enq_Code", item.Party_Enq_Code ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Party_item_name", item.Party_item_name ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Dsg_code", item.Dsg_code ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Units ", item.Units ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Size", item.Size ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Qtydosg_unit_perpack ", item.Qtydosg_unit_perpack ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@No_of_pack ", item.No_of_pack ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Total_unit ", item.Total_unit ?? (object)DBNull.Value);
                            if (string.IsNullOrEmpty(item.Imagedata))
                            {
                                command.Parameters.AddWithValue("@Product_Image", new byte[0]);
                            }
                            else
                            {

                                byte[] pi = Convert.FromBase64String(item.Imagedata);
                                command.Parameters.AddWithValue("@Product_Image", pi);
                            }
                            //SqlParameter binaryParameter = command.Parameters.Add("@Product_Image ", SqlDbType.VarBinary, -1);
                            //binaryParameter.Value = (object)item.Product_Image ?? DBNull.Value;
                            command.Parameters.AddWithValue("@qtation_status", item.qtation_status ?? "Quotation Is Not Given");
                            command.Parameters.AddWithValue("@item_status", item.item_status ?? "Item Not Add In Sale Order");
                            command.Parameters.AddWithValue("@Enquiry_Status", item.Enquiry_Status ?? "Pending");
                            //SqlParameter dateParameter = new SqlParameter("@date", SqlDbType.DateTime);
                            //dateParameter.Value = (object)item.date ?? DBNull.Value;
                            //command.Parameters.AddWithValue("@date", dateParameter ?? (object)DBNull.Value);


                            //SqlParameter dateParameter = new SqlParameter("@date", SqlDbType.DateTime);
                            //dateParameter.Value = (object)item.date ?? DBNull.Value;
                            //command.Parameters.AddWithValue("@date", dateParameter.Value); // Only this line is needed


                            result = (int)command.ExecuteScalar();

                            if (result == 2)
                            {
                                totalRowsAffected++;
                            }
                            else if (result == 1)
                            {
                                return false;
                            }
                            else if (result == -1)
                            {
                                return false;
                            }

                        }
                    }

                    if (totalRowsAffected > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }



            //Connection();
            //using (SqlCommand cmd = new SqlCommand("AddNewProductDetails", con))
            //{
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("@Name", model.Name);
            //    cmd.Parameters.AddWithValue("@Description", model.Description);
            //    cmd.Parameters.AddWithValue("@UnitPrice", model.UnitPrice);
            //    cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
            //    con.Open();
            //    bool retResult = cmd.ExecuteNonQuery() >= 1;
            //    con.Close();
            //    return retResult;
            //}
        }

        public async Task<bool> UpdateInquiryItem(InquiryItemUpdate model)
        {
            Connection();
            con.Open();

            using (SqlCommand command = new SqlCommand("UPDATE e_qutation SET qtation_status = @Value1, " +
                  "Party_item_name = @Value4, " +
                    "gen_code = @Value6, " +
                 "Dsg_code = @Value7, " +
                  "Packing_type = @Value8, " +
                   "Size = @Value9, " +
                      "Units = @Value10, " +
                 "Qtydosg_unit_perpack = @Value11, " +
                  "No_of_pack = @Value12, " +
                   "Total_unit = @Value13, " +
                    "Qty_shipr = @Value14, " +
                      "Wt_of_shipr = @Value15, " +
                 "Vol_per_shipr = @Value16, " +
                  "length = @Value17, " +
                   "breath = @Value18, " +
                      "height = @Value19, " +
                   "Tot_shipr = @Value20, " +
                    "Tot_wt = @Value21, " +
                      "Tot_vol = @Value22, " +
                 "FormulationPerUnit = @Value23, " +
                  "ComponentPerUnit = @Value24, " +
                   "Ex_fact_price = @Value25, " +
                     "Total_val = @Value26, " +
                   "Val_in_usd = @Value27 " +
                "WHERE Party_Enq_Code = @Party_Enq_Code and item_code=@item_code", con))
            {
                command.Parameters.AddWithValue("@Value1", "Quotation Is Given");
                command.Parameters.AddWithValue("@Value4", model.PartyItemName);
                command.Parameters.AddWithValue("@Value6", model.CorrGenName);
                command.Parameters.AddWithValue("@Value7", model.DrugDossageForm);
                command.Parameters.AddWithValue("@Value8", model.PackingType);
                command.Parameters.AddWithValue("@Value9", model.Size);
                command.Parameters.AddWithValue("@Value10",model.Unit);
                command.Parameters.AddWithValue("@Value11",model.QtyPerPack);
                command.Parameters.AddWithValue("@Value12",model.NoOfPack);
                command.Parameters.AddWithValue("@Value13",model.TotalUnit);
                command.Parameters.AddWithValue("@Value14",model.Qty_shipr);
                command.Parameters.AddWithValue("@Value15",model.WtOfShipr);
                command.Parameters.AddWithValue("@Value16",model.VolPershipr);
                command.Parameters.AddWithValue("@Value17",model.length);
                command.Parameters.AddWithValue("@Value18",model.breath);
                command.Parameters.AddWithValue("@Value19",model.height);
                command.Parameters.AddWithValue("@Value20",model.tot_shipr);
                command.Parameters.AddWithValue("@Value21",model.tot_wt);
                command.Parameters.AddWithValue("@Value22",model.tot_vol);
                command.Parameters.AddWithValue("@Value23",model.FormulationUnit);
                command.Parameters.AddWithValue("@Value24",model.ComponentPerUnit);
                command.Parameters.AddWithValue("@Value25",model.Ex_fact_price);
                command.Parameters.AddWithValue("@Value26",model.Total_val);
                command.Parameters.AddWithValue("@Value27",model.Val_in_usd);
                command.Parameters.AddWithValue("@Party_Enq_Code", model.InquiryCode);
                command.Parameters.AddWithValue("@item_code", model.ItemCode);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #region Costing
        public async Task<CostingResult> GetCosting(string WorkOrder)
        {

            //string work_order = "";
            //string gen_code = "";
            //string dsg_form_code = "";
            //string TotaoNoUnits = "";
            Connection();
            List<DataGrid2> prod = new List<DataGrid2>();
            DataGrid2 grid2 = new DataGrid2();
            con.Open();
            using (SqlCommand cmd = new SqlCommand("SaleOrder", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@work_order_no", WorkOrder);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                     grid2.work_order_no = reader[0].ToString();
                     grid2.sale_ordr_date =reader[1].ToString();
                     grid2.Category =reader[2].ToString();
                     grid2.dsg_form =reader[5].ToString();
                     grid2.cor_gen_name =reader[6].ToString();
                     grid2.gen_code =reader[7].ToString();
                     //grid2.CorrGenericName =reader["CorrGenericName"].ToString();
                     grid2.combi_code =reader[14].ToString();
                     grid2.item_status=reader[16].ToString();
                     grid2.Size=reader[9].ToString();
                     grid2.QtyOfDossageUnitPerPack= reader[10].ToString();
                     grid2.NoOfPacks= reader[11].ToString();
                     grid2.TotalNoOf= reader[12].ToString();
                     grid2.desc_code=reader[17].ToString();
                     grid2.dsg_code = reader[21].ToString();
       
                }
                reader.Close();
            }
            desc_code = grid2.desc_code;
            dsg_code = grid2.dsg_code;
            gen_code = grid2.gen_code;
            Size = grid2.Size;
            work_order = grid2.work_order_no;
            ComponentCosting(grid2.work_order_no, grid2.item_status);


            if (dsg_code == "DSG012")
            {
                FillFormulationDetails(gen_code,Size);
            }
            else if (dsg_code == "DSG002")
            {
                FillFormulationDetailsInjectible(gen_code, Size);
            }
            else if (dsg_code == "DSG005" || dsg_code == "DSG004" || dsg_code == "DSG009" || dsg_code == "DSG010")
            {
                FillFormulationDetailsLiquidOral(gen_code, Size);
            }
            else if (dsg_code == "DSG003")
            {
                FillFormulationDetailsCream(gen_code, Size);
            }

            else
            {
                Field11txtSize = "";
                Field3txtUnitSize = "";
                Field4txtWeightOfUnit = "";
                Field10txtPowderWeightPerUnit = "";
                Field5txtWeightUnit = "";
                Field9txtMuliplierFactor = "";
                Field7txtTotNoUnitInBatch = "";
            }
            FormulationCosting();
            con.Close();
            return costing;
        }

        private void FillFormulationDetailsLiquidOral(string gen_code, string size)
        {
            Field11txtSize = "";
            Field3txtUnitSize = "";
            Field4txtWeightOfUnit = "";
            Field10txtPowderWeightPerUnit = "";
            Field5txtWeightUnit = "";
            Field9txtMuliplierFactor = "";
            Field7txtTotNoUnitInBatch = "";

            //dtg_all.DataSource = null;
            DataTable dtF = new DataTable();
            dtF.Clear();
            using (SqlCommand cmd = new SqlCommand("FormulationLiquidOral", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@gen_code", gen_code);
                SqlDataReader reader = cmd.ExecuteReader();
                dtF.Load(reader);
                reader.Close();
                Field11txtSize = dtF.Rows[0][1].ToString();
                Field3txtUnitSize = dtF.Rows[0][3].ToString();
                Field4txtWeightOfUnit = dtF.Rows[0][2].ToString();
                Field10txtPowderWeightPerUnit = dtF.Rows[0][13].ToString();
                Field5txtWeightUnit = dtF.Rows[0][13].ToString();
                Field9txtMuliplierFactor = Math.Round(Convert.ToDouble(dtF.Rows[0][15]), 0).ToString();
                Field7txtTotNoUnitInBatch = dtF.Rows[0][12].ToString();

            }
        }
        private void FillFormulationDetailsCream(string gen_code, string size)
        {
            Field11txtSize = "";
            Field3txtUnitSize = "";
            Field4txtWeightOfUnit = "";
            Field10txtPowderWeightPerUnit = "";
            Field5txtWeightUnit = "";
            Field9txtMuliplierFactor = "";
            Field7txtTotNoUnitInBatch = "";

            //dtg_all.DataSource = null;
            DataTable dtF = new DataTable();
            dtF.Clear();
            double sizeinteger = 0.0;
            sizeinteger =Convert.ToDouble(Regex.Replace(size, "[^0-9]", ""));
            using (SqlCommand cmd = new SqlCommand("FormulationLiquidOral", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@gen_code", gen_code);
                SqlDataReader reader = cmd.ExecuteReader();
                dtF.Load(reader);
                reader.Close();
                Field11txtSize = dtF.Rows[0][1].ToString();
                Field3txtUnitSize = dtF.Rows[0][3].ToString();
                Field4txtWeightOfUnit = dtF.Rows[0][2].ToString();
                Field10txtPowderWeightPerUnit = dtF.Rows[0][13].ToString();
                Field5txtWeightUnit = dtF.Rows[0][13].ToString();
                Field9txtMuliplierFactor = Math.Round(Convert.ToDouble(dtF.Rows[0][15]), 0).ToString();
                //Field7txtTotNoUnitInBatch = dtF.Rows[0][12].ToString();
                Field7txtTotNoUnitInBatch = Math.Round(int.Parse(dtF.Rows[0][4].ToString()) * 1000 / sizeinteger, 0).ToString();


            }
        }


        private void FillFormulationDetailsInjectible(string gen_code, string size)
        {
            DataTable dtbl = new DataTable();
           
            using (SqlCommand cmd = new SqlCommand("select Drug_Licence,drug_dosg_form from finished_product where gen_code =@gen_code", con))
            {
                cmd.Parameters.AddWithValue("@gen_code", gen_code);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    dtbl.Load(reader);
                    reader.Close();
                }

            }
                if (dtbl.Rows.Count > 0)
                {
                    drugLicense = dtbl.Rows[0][0].ToString();
                    dsgcode = dtbl.Rows[0][1].ToString();
                }
               

               
                    Field11txtSize = "";
                    Field3txtUnitSize = "";
                    Field4txtWeightOfUnit = "";
                    Field10txtPowderWeightPerUnit = "";
                    Field5txtWeightUnit = "";
                    Field9txtMuliplierFactor = "";
                    Field7txtTotNoUnitInBatch = "";

            DataTable dtF = new DataTable();
            dtF.Clear();
           
            if (!(drugLicense == "Betalactum Sterile Powder For Injection" ||
                         drugLicense == "Cephalosporin Sterile Powder For Injection" ||
                         drugLicense == "General Sterile Powder For Injection" ||
                         drugLicense == "Cytotoxic Sterile Powder For Injection" ||
                         drugLicense == "Hormones Sterile Powder For Injection"))
            {
               
                using (SqlCommand cmd = new SqlCommand("Formulation002", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@gen_code", gen_code);
                    SqlDataReader reader = cmd.ExecuteReader();

                    dtF.Load(reader);
                    reader.Close();
                    double sizeinteger = 0;
                    if (size.Contains("ml"))
                    {
                        string[] parts = size.Split("ml");
                        sizeinteger = double.Parse(parts[0]);
                    }
                    else if (size.Contains("PCS"))
                    {
                        string[] parts = size.Split("PCS");
                        sizeinteger = double.Parse(parts[0]);
                    }

                    Field11txtSize = dtF.Rows[0][1].ToString();
                    Field3txtUnitSize = dtF.Rows[0][3].ToString();
                    Field4txtWeightOfUnit = dtF.Rows[0][2].ToString();
                    Field10txtPowderWeightPerUnit = sizeinteger.ToString();
                    Field5txtWeightUnit = sizeinteger.ToString();
                    Field9txtMuliplierFactor = Math.Round(sizeinteger / int.Parse(dtF.Rows[0][6].ToString()), 2).ToString();
                    Field7txtTotNoUnitInBatch = Math.Round(int.Parse(dtF.Rows[0][4].ToString()) * 1000 / sizeinteger, 0).ToString();

                }       
            }
        }


        private void FillFormulationDetails(string gen_code, string size)
        {
            Field11txtSize = "";
            Field3txtUnitSize = "";
            Field4txtWeightOfUnit = "";
            Field10txtPowderWeightPerUnit = "";
            Field5txtWeightUnit = "";
            Field9txtMuliplierFactor = "";
            Field7txtTotNoUnitInBatch = "";

            //dtg_all.DataSource = null;
            DataTable dtF = new DataTable();
            dtF.Clear();
         
            using (SqlCommand cmd = new SqlCommand("Formulation012", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@gen_code", gen_code);
                SqlDataReader reader = cmd.ExecuteReader();
              
                dtF.Load(reader);
                reader.Close();
                Field11txtSize = dtF.Rows[0][1].ToString();
                    Field3txtUnitSize = dtF.Rows[0][3].ToString();
                    Field4txtWeightOfUnit = dtF.Rows[0][2].ToString();
                    Field10txtPowderWeightPerUnit = Math.Round(Convert.ToDouble(dtF.Rows[0][16]), 1).ToString();
                    Field5txtWeightUnit = dtF.Rows[0][19].ToString();
                    Field9txtMuliplierFactor = Math.Round(Convert.ToDouble(dtF.Rows[0][15]), 0).ToString();
                    Field7txtTotNoUnitInBatch = Math.Round(Convert.ToDouble(dtF.Rows[0][17]), 0).ToString();
                
            }
           
        }


        private void ComponentCosting(string work_order_no,string itemSts)
        {



            //Dim d As New DataTable
            //Dim itemSts As String
            //Connection();
            //string itemSts1 = itemSts;
            if (itemSts == "NONE")
            {
                 d= componentCostingForNonCombi(work_order_no);
            }
            else
            {
               
                //con.Open();
                var command = "SELECT TOP 1 New_combi_code FROM sale_order WHERE work_order_no = @work_order_no AND Revised_status = 'Final';";
                using (SqlCommand cmd = new SqlCommand(command, con))
                {
                    cmd.Parameters.AddWithValue("@work_order_no", work_order_no);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        combicode = reader[0].ToString();
                    }
                    reader.Close();

                }    
                d=componentCostingForCombi(combicode);
            }

            for (int i = 0; i < d.Rows.Count; i++)
            {
                for (int j = 2; j <= 4; j++)
                {
                    if (d.Rows[i][j] == DBNull.Value)
                    {
                        d.Rows[i][j] = "NA";
                    }
                }
            }

            for (int i = 0; i < d.Rows.Count; i++)
            {
                for (int j = 5; j <= 6; j++)
                {
                    if (d.Rows[i][j] == DBNull.Value)
                    {
                        d.Rows[i][j] = 0;
                    }
                }
            }

            if (d.Rows.Count > 0)
            {
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    if (!DBNull.Value.Equals(d.Rows[i][5]) && !DBNull.Value.Equals(d.Rows[i][6]))
                    {
                        d.Rows[i][7] = Convert.ToDecimal(d.Rows[i][5]) * Convert.ToDecimal(d.Rows[i][6]);
                    }
                    //if (!DBNull.Value.Equals(d.Rows[i][9]))
                    //{
                    //    d.Rows[i][13] = "Purchase";
                    //    d.Rows[i][12] = "";
                    //}
                    //else if (!DBNull.Value.Equals(d.Rows[i][10]))
                    //{
                    //    d.Rows[i][13] = "PO";
                    //}
                }
            }

            if (d.Rows.Count > 0)
            {
                string WO="";
                DataTable dtt = new DataTable();
                DataTable dtt2 = new DataTable();
                int orderedQty = 0;
                int determinedQty = 0;
                int NoOfKits = 0;

                try
                {
                    if (itemSts == "NONE")
                    {
                        //Connection();
                        //con.Open();
                        using (SqlCommand cmd = new SqlCommand("select s.work_order_no,s.total_units as 'Ordered Quantity',d.determind_qty_part1 as 'Determined Quantity' from sale_order s left join tbl_determination d on d.work_order_no = s.work_order_no WHERE s.work_order_no = @work_order_no and s.revised_status='final'and d.determin_status ='Final'", con))
                        {
                            //cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@work_order_no", work_order_no);
                            SqlDataReader reader = cmd.ExecuteReader();

                            dtt.Load(reader);
                            reader.Close();

                        }
                        if (dtt.Rows.Count > 0 && Convert.ToInt32(dtt.Rows[0][1]) > 0 && Convert.ToInt32(dtt.Rows[0][2]) > 0)
                        {
                            WO = dtt.Rows[0][0].ToString();
                            orderedQty = Convert.ToInt32(dtt.Rows[0][1]);
                            determinedQty = Convert.ToInt32(dtt.Rows[0][2]);
                        }
                        else
                        {
                            using (SqlCommand cmd = new SqlCommand("select s.work_order_no,s.total_units as 'Ordered Quantity' from sale_order s WHERE s.work_order_no = @work_order_no and s.revised_status='final'", con))
                            {
                                //cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@work_order_no", work_order_no);
                                SqlDataReader reader = cmd.ExecuteReader();

                                dtt2.Load(reader);
                                reader.Close();

                            }

                            if (dtt2.Rows.Count > 0)
                            {
                                WO = dtt2.Rows[0][0].ToString();
                                orderedQty = Convert.ToInt32(dtt2.Rows[0][1]);
                                determinedQty = Convert.ToInt32(Math.Round(Convert.ToDecimal(dtt2.Rows[0][1]) * 1.05m, 2)); // ordered qty plus 5 % of ordered qty
                            }

                        }
                        d = QtyofShrinkSleev(desc_code, determinedQty, d);


                        for (int i = 0; i < d.Rows.Count - 2; i++)
                        {
                            if (orderedQty != 0)
                            {
                                d.Rows[i].SetField<decimal>(8, Math.Round(Convert.ToDecimal(d.Rows[i][7]) / orderedQty, 5));
                            }
                        }
                        d = addrowwithtotal(d);
                        if (orderedQty != 0)
                        {
                            ccpoq = Math.Round(ComponentTotal / orderedQty, 2); // total component rate/ ordered qty
                        }
                        if (determinedQty != 0)
                        {
                            ccpdq = Math.Round(ComponentTotal / determinedQty, 2); // total component rate/ determined qty
                        }
                        DataRow NewRow = d.NewRow();
                        NewRow[0] = "Work Order No.- " + WO.ToString();
                        NewRow[1] = "Ordered Qty:- " + orderedQty.ToString();
                        NewRow[2] = "Determined Qty:-" + determinedQty.ToString();
                        NewRow[3] = "Component Cost/Ordered Qty:- " + ComponentTotal.ToString() + "/" + orderedQty.ToString() + "=" + ccpoq + "   Component Cost/Determined Qty:-" + ComponentTotal.ToString() + "/" + determinedQty.ToString() + "=" + ccpdq;
                        NewRow[7] = 0.0m;
                        NewRow[8] = 0.0m;
                        NewRow[13] = "";
                        d.Rows.Add(NewRow);
                        d.AcceptChanges();
                    }
                    else
                    {
                        //con.Open();
                        using (SqlCommand cmd = new SqlCommand("select distinct combi_code , total_qty_of_combi_in_order as 'No. of Combi Kits' from sale_order_combi_detail where combi_code =@combicode and Revised_status ='final'", con))
                        {
                            //cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@combicode", combicode);
                            SqlDataReader reader = cmd.ExecuteReader();

                            dtt.Load(reader);
                            reader.Close();

                        }
                        if (dtt.Rows.Count > 0 && Convert.ToInt32(dtt.Rows[0][1]) > 0)
                        {
                            NoOfKits = Convert.ToInt32(dtt.Rows[0][1]);
                        }
                        d = QtyofShrinkSleev(desc_code, NoOfKits, d);

                        for (int i = 0; i < d.Rows.Count - 2; i++)
                        {
                            if (NoOfKits != 0)
                            {
                                d.Rows[i].SetField<decimal>(8, Math.Round(Convert.ToDecimal(d.Rows[i][7]) / NoOfKits, 5));
                            }
                        }
                        d = addrowwithtotal(d);
                        if (NoOfKits != 0)
                        {
                            ccpoq = Math.Round(ComponentTotal / NoOfKits, 2); // total component rate / Ordered Qty or / Pack
                        }
                        DataRow NewRow = d.NewRow();
                        NewRow[0] = "Combi Code:- " + dtt.Rows[0][0].ToString();
                        NewRow[1] = "No. of Combi Kits:- " + dtt.Rows[0][1].ToString();
                        NewRow[2] = "Component Cost/Pack:- " + ccpoq;
                        NewRow[7] = 0.0m;
                        NewRow[8] = 0.0m;
                        NewRow[13] = "";
                        d.Rows.Add(NewRow);
                        d.AcceptChanges();
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

               
            }
        }

       
        private DataTable addrowwithtotal(DataTable d)
            {
            try
            {
                decimal QtyTotal = Convert.ToDecimal(d.Compute("Sum(RequiredQty)", ""));
                decimal AmtTotal = Convert.ToDecimal(d.Compute("Sum(Amount)", ""));
                ComponentTotal = Convert.ToDouble(AmtTotal);
                DataRow MyNewRow = d.NewRow();
                MyNewRow[0] = "Total";
                MyNewRow[5] = QtyTotal;
                MyNewRow[7] = AmtTotal;
                MyNewRow[8] = 0.0m;
                MyNewRow[13] = "";
                d.Rows.Add(MyNewRow);
                d.AcceptChanges();
                return d;
            }
            catch (Exception ex)
            {

                throw ex;
            } 
                
            }
        

        private DataTable componentCostingForNonCombi(string work_order_no)
        {
            //Connection();
          
               // con.Open();
            using (SqlCommand cmd = new SqlCommand("Noncombi", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@work_order_no", work_order_no);
                SqlDataReader reader = cmd.ExecuteReader();

                d.Load(reader);
                reader.Close();
                //while (reader.Read())
                //{

                //}
                //adp.Fill(d);
            }
                return d;
            
        }
        private DataTable componentCostingForCombi(string combicode)
        {
            //Connection();
            //con.Open();
            using (SqlCommand cmd = new SqlCommand("CombiComponentList", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@combi_code", combicode);
                SqlDataReader reader = cmd.ExecuteReader();
                d.Load(reader);
                reader.Close();
            }

            DataTable dt1 = new DataTable();
            Fill_combiDetails(dt1);
            return d;
            
        }

        private DataTable Fill_combiDetails(DataTable dtt)
        {
            dtt.Clear();
            //Connection();
            ListOfGenCodes.Clear();
            ListOfworkorder.Clear();
            //con.Open();
            using (SqlCommand cmd = new SqlCommand("ListFillforCombi", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@combi_code", combicode);
                SqlDataReader reader = cmd.ExecuteReader();
                dtt.Load(reader);
                reader.Close();
            } 
            if (dtt.Rows.Count > 0)
            {
             
                foreach (DataRow r in dtt.Rows)
                {
                    ListOfGenCodes.Add(r["Item Gen Code"].ToString()); //## gencodes for combi kit formulation
                }
                foreach (DataRow r in dtt.Rows)
                {
                    ListOfworkorder.Add(r["work_order_no"].ToString()); //## workorders for combi kit formulation
                }
            }
            return dtt;
        }


        private DataTable QtyofShrinkSleev(string desc_code, int determinedQty, DataTable d)
        {
            DataTable dtpo = new DataTable();
            using (SqlCommand cmd = new SqlCommand("SELECT Units AS TotalQtyOfShrink FROM GetShrinkSleevQuantity(@desc_code)", con))
            {
                cmd.Parameters.AddWithValue("@desc_code", desc_code);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    dtpo.Load(reader);
                    reader.Close();
                }
            }
            const decimal PriceOfSHRINKSLEEVE = 1.25m;
            decimal TotalQtyOfShrink = 1;

            // Shrink Sleev Quantity
            if (dtpo.Rows.Count > 0)
            {
                foreach (DataRow DataRow in dtpo.Rows)
                {
                    TotalQtyOfShrink *= Convert.ToDecimal(DataRow[0]);
                }
            }

            if (d.Rows.Count > 0)
            {
                for (int index = 0; index < d.Rows.Count; index++)
                {
                    if (d.Rows[index][0].ToString() == "SHRINK SLEEVE" && TotalQtyOfShrink > 0)
                    {
                        d.Rows[index][5] = TotalQtyOfShrink;
                        TotalQtyOfShrink = Math.Round(Convert.ToDecimal(determinedQty) / TotalQtyOfShrink, 5);
                        d.Rows[index][6] = PriceOfSHRINKSLEEVE;
                        d.Rows[index][7] = TotalQtyOfShrink * PriceOfSHRINKSLEEVE;
                        d.Rows[index][3] = "(DeterminedQty / Units In Shrink ) X 1.25";
                    }
                }
            }

            return d;
        }


        private void FormulationCosting()
        {
           //this.Cursor = Cursors.WaitCursor;
                dgFormulation.Clear();
                DataTable dt = new DataTable();
                DataTable finaldt = new DataTable();
                DataTable dtresult = new DataTable();
                _totalFormulation = 0.0m;
                TotalFormulationRate_perpack_perWo = 0.0;
                TotalFormulationRate_perpack_perD = 0.0;
                int index = 0;
                finaldt.Clear();
                wono = "";
                GENCODE = "";
                // Formulation Costing for combi
                if (ListOfGenCodes.Count > 0)
                {
                    for (index = 0; index < ListOfGenCodes.Count; index++)
                    {
                        GENCODE = ListOfGenCodes[index].ToString();
                        wono = ListOfworkorder[index].ToString();
                        FormulationCostingforGenCode(dt, GENCODE);
                        finaldt.Merge(dt);
                    }
                    DataRow NewRow = finaldt.NewRow();
                    NewRow[9] = _totalFormulation;
                    finaldt.Rows.Add(NewRow);
                    finaldt.AcceptChanges();

                    dtresult = finaldt.Copy();
                    if (dtresult.Rows.Count > 0)
                    {
                        dgFormulation = dtresult.Copy();
                    }
                    FillTotals();
                    completecostingforcombi();
                }
                else    // Formulation Costing for Non Combi
                {
                    if (!string.IsNullOrEmpty(gen_code) && gen_code != "0")
                    {
                        FormulationCostingforGenCode(dt, gen_code);
                    }
                    dtresult = dt.Copy();
                    if (dtresult.Rows.Count > 0)
                    {
                        dgFormulation = dtresult.Copy();
                    }
                    FillTotals();
                    CompleteCosting(work_order, gen_code);
                }
        }

        private void CompleteCosting(string work_order, string gen_code)
        {
           
               
                dgCompleteCosting.Clear();
                StringBuilder sb = new StringBuilder();
                DataSet ds = new DataSet();
                DataTable dd = new DataTable();
                DataTable df = new DataTable();
                sb.Clear();
                ds.Clear();
                dd.Clear();
                df.Clear();

            //Connection();
            //con.Open();
            using (SqlCommand cmd = new SqlCommand("ccfornoncombi", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@work_order_no", work_order);
                SqlDataReader reader = cmd.ExecuteReader();
                
                    dd.Load(reader);
                reader.Close();

            }

            using (SqlCommand cmd = new SqlCommand("noncombiformulation", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@gen_code", gen_code);
                SqlDataReader reader = cmd.ExecuteReader();
                df.Load(reader);
                reader.Close();
            }
                if (dd.Rows.Count == 0)
                {


                using (SqlCommand cmd = new SqlCommand("noncombi2", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@work_order", work_order);
                    SqlDataReader reader = cmd.ExecuteReader();
                    dd.Load(reader);
                    reader.Close();
                }

                
                    if (dd.Rows.Count > 0)
                    {
                        decimal determinedqty = Math.Round(Convert.ToDecimal(dd.Rows[0][1]) * 1.05m, 2); // ordered qty plus 5 % of ordered qty
                        dd.Rows[0][2] = determinedqty;
                    }
                }
                GetCompeteCostingIngrid(dd, df);
            
            
        }
        private void GetCompeteCostingIngrid(DataTable dd, DataTable df)
        {
            try
            {
                if (dd != null && dd.Rows.Count > 0)
                {
                    dd.Rows[0][8] = Math.Round(Convert.ToDecimal(_totalFormulation), 2);  //#### Total Formulation Cost
                                                                                          //#############################  FOR LIQUID ORALS             
                                                                                          //Formulation Rate Per Unit-6 =  ( Formulation Total-1 / (Lot Size-3 * Lot Size-3) * Size-2) * (Determined Quantity-4  / ordered qty 5)   '#####  Formulation Total per unit per work order 
                    decimal ColFormulationTotal1 = Convert.ToDecimal(dd.Rows[0]["Formulation Total"]);
                    decimal ColSize2 = Convert.ToDecimal(dd.Rows[0]["Size"]);
                    decimal ColLotSize3;
                    decimal ColDetermQty4 = Convert.ToDecimal(dd.Rows[0]["Determined Quantity"]);
                    decimal ColOrderedQty5 = Convert.ToDecimal(dd.Rows[0]["Ordered Quantity"]);
                    decimal CostperbtCalFormula;
                    decimal ColFormulationRatePerUnit6;
                    //if (!string.IsNullOrEmpty(df.Rows[0]["tot_no_of_unit"].ToString()))
                    if (Convert.ToDecimal(df.Rows[0]["tot_no_of_unit"]) == 0 || string.IsNullOrEmpty(df.Rows[0]["tot_no_of_unit"].ToString()))
                        df.Rows[0]["tot_no_of_unit"] = 1;

                    if (df.Rows[0]["unit"].ToString().Equals("LTR", StringComparison.OrdinalIgnoreCase) || df.Rows[0]["unit"].ToString().Equals("KG", StringComparison.OrdinalIgnoreCase)) //If UOM is LTR or KG
                    {
                        ColLotSize3 = Convert.ToDecimal(df.Rows[0]["lot_size"]) * 1000;

                        CostperbtCalFormula = Convert.ToDecimal(ColFormulationTotal1 / Convert.ToDecimal(Field7txtTotNoUnitInBatch));
                        decimal CostperDeterminQty = CostperbtCalFormula * ColDetermQty4;
                        decimal CalFormulation = CostperDeterminQty / ColOrderedQty5;
                        ColFormulationRatePerUnit6 = CalFormulation;

                        dd.Rows[0][7] = Math.Round(ColFormulationRatePerUnit6, 6);  //####       Formulation per Unit per workOrder
                    }
                    else
                    {
                        ColLotSize3 = Convert.ToDecimal(df.Rows[0]["lot_size"]); //If UOM is PCS
                        ColFormulationRatePerUnit6 = Convert.ToDecimal((ColFormulationTotal1 / ColLotSize3 * ColSize2) * (ColDetermQty4 / ColOrderedQty5));
                        dd.Rows[0][7] = Math.Round(ColFormulationRatePerUnit6, 6);
                    }

                    drugLicense = drugLicense != null ? drugLicense : "";
                    dsgcode = dsgcode != null ? dsgcode : "";




                    if (string.IsNullOrEmpty(dsgcode))
                    {
                        dsgcode = dsg_code;
                    }

                    if (drugLicense == "Betalactum Sterile Powder For Injection" || drugLicense == "Cephalosporin Sterile Powder For Injection" || drugLicense == "Betalactum" || drugLicense == "General Sterile Powder For Injection" || dsgcode == "DSG001" || dsgcode == "DSG006" || drugLicense == "Cytotoxic Sterile Powder For Injection" || drugLicense == "Hormones Sterile Powder For Injection")
                    {
                        if (!string.IsNullOrEmpty(df.Rows[0]["lot_size"].ToString()) && Convert.ToDecimal(df.Rows[0]["lot_size"]) != 0 && Convert.ToDecimal(dd.Rows[0]["Size"]) != 0 && !string.IsNullOrEmpty(dd.Rows[0]["Size"].ToString()))
                        {
                            dd.Rows[0]["Formulation Rate Per Determined Qty"] = Math.Round(ColFormulationTotal1 / Convert.ToDecimal(df.Rows[0]["lot_size"]) * Convert.ToDecimal(dd.Rows[0]["Size"]), 4);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(df.Rows[0]["tot_no_of_unit"].ToString()) && Convert.ToDecimal(df.Rows[0]["tot_no_of_unit"]) != 0 && Convert.ToDecimal(dd.Rows[0]["Size"]) != 0 && !string.IsNullOrEmpty(dd.Rows[0]["Size"].ToString()))
                        {
                            if (dsgcode == "DSG007")
                            {
                                dd.Rows[0]["Formulation Rate Per Determined Qty"] = Math.Round(ColFormulationTotal1 / Convert.ToDecimal(df.Rows[0]["tot_no_of_unit"]) * Convert.ToDecimal(dd.Rows[0]["Size"]), 4); //##### Formulation Total / Determined qty
                            }
                            else
                            {
                                dd.Rows[0]["Formulation Rate Per Determined Qty"] = Math.Round(ColFormulationTotal1 / Convert.ToDecimal(Field7txtTotNoUnitInBatch), 4);
                            }
                        }
                    }

                    //string dosage = ((System.Data.DataRowView)cmb_dossage.SelectedItem).Row.ItemArray[0].ToString();
                    string dosage = dsg_code;
                    switch (dosage)
                    {
                        //###################Calculation for DrySurp only############
                        case "DSG012":
                            CostperbtCalFormula = Convert.ToDecimal(ColFormulationTotal1 / Convert.ToDecimal(Field7txtTotNoUnitInBatch));
                            decimal CostperDeterminQty = CostperbtCalFormula * ColDetermQty4;
                            decimal CalFormulation = CostperDeterminQty / ColOrderedQty5;
                            ColFormulationRatePerUnit6 = CalFormulation;

                            dd.Rows[0][7] = Math.Round(ColFormulationRatePerUnit6, 6);
                            break;

                        //###################Calculation for INJECTABLE,LIQUIDEXTERNAL, LIQUIDORALS  only############
                        case "DSG002": //Case constINJECTABLE
                            if (drugLicense == "Betalactum Sterile Powder For Injection" || drugLicense == "Cephalosporin Sterile Powder For Injection" || drugLicense == "Betalactum" || drugLicense == "General Sterile Powder For Injection" || dsgcode == "DSG001" || dsgcode == "DSG006" || drugLicense == "Cytotoxic Sterile Powder For Injection" || drugLicense == "Hormones Sterile Powder For Injection")
                            {
                                if (!string.IsNullOrEmpty(df.Rows[0]["lot_size"].ToString()) &&
                                Convert.ToDecimal(df.Rows[0]["lot_size"]) != 0 && Convert.ToDecimal(dd.Rows[0]["Size"]) != 0 && !string.IsNullOrEmpty(dd.Rows[0]["Size"].ToString()))
                                {
                                    ColFormulationRatePerUnit6 = Convert.ToDecimal((ColFormulationTotal1 / Convert.ToDecimal(df.Rows[0]["lot_size"])) * (ColDetermQty4 / ColOrderedQty5));
                                    dd.Rows[0][7] = Math.Round(ColFormulationRatePerUnit6, 6);
                                }
                            }
                            else
                            {
                                ColFormulationRatePerUnit6 = Convert.ToDecimal((ColFormulationTotal1 / Convert.ToDecimal(Field7txtTotNoUnitInBatch)) * (ColDetermQty4 / ColOrderedQty5));
                                dd.Rows[0][7] = Math.Round(ColFormulationRatePerUnit6, 6);
                            }
                            break;

                        case "DSG005": //Case 
                            ColFormulationRatePerUnit6 = Convert.ToDecimal((ColFormulationTotal1 / Convert.ToDecimal(Field7txtTotNoUnitInBatch)) * (ColDetermQty4 / ColOrderedQty5));
                            dd.Rows[0][7] = Math.Round(ColFormulationRatePerUnit6, 6);
                            break;

                        case "DSG003": //Case 
                            ColFormulationRatePerUnit6 = Convert.ToDecimal((ColFormulationTotal1 / Convert.ToDecimal(Field7txtTotNoUnitInBatch)) * (ColDetermQty4 / ColOrderedQty5));
                            dd.Rows[0][7] = Math.Round(ColFormulationRatePerUnit6, 6);
                            break;

                        case "DSG004": //Case 
                            ColFormulationRatePerUnit6 = Convert.ToDecimal((ColFormulationTotal1 / Convert.ToDecimal(Field7txtTotNoUnitInBatch)) * (ColDetermQty4 / ColOrderedQty5));
                            dd.Rows[0][7] = Math.Round(ColFormulationRatePerUnit6, 6);
                            break;

                        case "DSG009": //Case 
                            ColFormulationRatePerUnit6 = Convert.ToDecimal((ColFormulationTotal1 / Convert.ToDecimal(Field7txtTotNoUnitInBatch)) * (ColDetermQty4 / ColOrderedQty5));
                            dd.Rows[0][7] = Math.Round(ColFormulationRatePerUnit6, 6);
                            break;

                        case "DSG010": //Case 
                            ColFormulationRatePerUnit6 = Convert.ToDecimal((ColFormulationTotal1 / Convert.ToDecimal(Field7txtTotNoUnitInBatch)) * (ColDetermQty4 / ColOrderedQty5));
                            dd.Rows[0][7] = Math.Round(ColFormulationRatePerUnit6, 6);
                            break;

                        default:
                            //Case constINJECTABLE, constLIQUIDEXTERNAL, constLIQUIDORALS
                            break;
                    }

                    dd.Rows[0][9] = Math.Round(Convert.ToDecimal(ComponentTotal), 2);               //#### Total Component Cost
                    dd.Rows[0][6] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(ComponentTotal) / Convert.ToDecimal(dd.Rows[0][1])), 4);   //#####  ComponentTotal/Ordered qty
                    dd.Rows[0][11] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(ComponentTotal) / Convert.ToDecimal(dd.Rows[0][2])), 2);   //##### ComponentTotal/Determined qty

                    //Starts ############## FOB / FOR Calculation
                    //************** FOB Calculation
                    if (dd.Rows[0]["category"].ToString() == "CAT0002")
                    {
                        //##fob_price / qty_per_pack * air_conv_rate
                        decimal price = Convert.ToDecimal(dd.Rows[0][4]) / Convert.ToDecimal(dd.Rows[0][13]);
                        dd.Rows[0][4] = Math.Round(price, 4);
                    }
                    else
                    {
                        //************** FOR Calculation
                        dd.Columns[3].ColumnName = "FOR Price In INR";
                        dd.Rows[0][3] = dd.Rows[0][15];

                        dd.Columns[4].ColumnName = "FOR Price Per Unit In INR";
                        decimal price = Convert.ToDecimal(dd.Rows[0][16]) / Convert.ToDecimal(dd.Rows[0][13]);  //###for_price/qty_per_pack  
                        dd.Rows[0][4] = Math.Round(price, 4);
                    }
                    //Ends ############## FOR / FOB Calculation
                    dd.Rows[0][5] = Math.Round(Convert.ToDecimal(dd.Rows[0][6]) + Convert.ToDecimal(dd.Rows[0][7]), 4); //#Total Cost per unit = component per unit + formulation per unit 
                    dgCompleteCosting = dd.Copy();
                    FillTotals(dd, "NonCombi");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }


        private void completecostingforcombi()
        {

            //Connection();
            DataTable dtmain = new DataTable();
            //con.Open();
            using (SqlCommand cmd = new SqlCommand("ccforcombi", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@combicode", combicode);
                SqlDataReader reader = cmd.ExecuteReader();
                dtmain.Load(reader);
                reader.Close();
            }


            //DataTable dc = new DataTable();
          
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("select distinct combi_code , total_qty_of_combi_in_order as 'No. of Combi Kits', 0 as 'FOB/FOR Price In INR', rate_per_pack, 0.000 AS '(C) = (A) + (B) Total Cost Per Pack ', 0.000 as '(A) Component Rate Per pack ', 0.000 as '(B) Formulation Rate Per Pack / WO', 0.000 as 'Formulation Total', 0.000 as ' Component Total', 0.00 AS 'Formulation Rate Per Pack/Determined Qty', rate_FOB_USD As 'FOB Price', rate_per_pack_xfactry  As 'FOR Price' , '' as Category from sale_order_combi_detail ");
            //sb.AppendLine(string.Format(" where combi_code ='{0}' and Revised_status ='final'", combicode));
            //DataTable dtmain = obj.GetDataTableProduction(sb.ToString());
            if (dtmain != null && dtmain.Rows.Count > 0)
            {
                dtmain.Rows[0][5] = Math.Round(ComponentTotal / Convert.ToDouble(dtmain.Rows[0][1]), 4);   //#####  ComponentTotal/No. of Combi Kits
                dtmain.Rows[0][6] = Math.Round(TotalFormulationRate_perpack_perWo, 2);  //#### Total Formulation Cost Per WO
                dtmain.Rows[0][7] = Math.Round(_totalFormulation, 2);  //#### Total Formulation Cost
                dtmain.Rows[0][8] = Math.Round(ComponentTotal, 2);               //#### Total Component Cost
                dtmain.Rows[0][9] = Math.Round(TotalFormulationRate_perpack_perD, 2);               //####  Total Formulation Cost Per Determined Qty

                object cat=null;
                //con.Open();
                var command = "select distinct category from sale_order where New_combi_code =@combicode; ";
                using (SqlCommand cmd = new SqlCommand(command, con))
                {
                    cmd.Parameters.AddWithValue("@combicode", combicode);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        cat = reader[0].ToString();
                    }
                    reader.Close();

                }







                //object cat = obj.GetScalarProduction(string.Format("select distinct category from sale_order where New_combi_code = '{0}'", combicode));
                if (cat != DBNull.Value && !string.IsNullOrEmpty(cat.ToString()))
                {
                    dtmain.Rows[0][12] = cat.ToString();
                }
                //Starts ############## FOB / FOR Calculation
                //************** FOB Calculation
                if (dtmain.Rows[0]["Category"].ToString() == "CAT0002")
                {
                    dtmain.Columns[2].ColumnName = "FOB Price In INR";
                    dtmain.Columns[3].ColumnName = "FOB Price Per Pack In INR";
                }
                else
                {
                    //************** FOR Calculation
                    dtmain.Columns[2].ColumnName = "FOR Price In INR";
                    dtmain.Columns[3].ColumnName = "FOR Price Per Pack In INR";
                }
                dtmain.Rows[0][2] = Math.Round(Convert.ToDecimal(dtmain.Rows[0][3]) * Convert.ToDecimal(dtmain.Rows[0][1]), 2);

                //Ends ############## FOR / FOB Calculation
                dtmain.Rows[0][4] = Math.Round(Convert.ToDecimal(dtmain.Rows[0][5]) + Convert.ToDecimal(dtmain.Rows[0][6]), 4); //#Total Cost per unit = component per unit + formulation per unit 
                dgCompleteCosting = dtmain.Copy();
                FillTotals(dtmain, "combi");
            }
          
        }



        private DataTable FormulationCostingforGenCode(DataTable dt, string gen_code)
        {
            dt.Clear();
            //Connection();
            if (drugLicense == "Betalactum Sterile Powder For Injection" || drugLicense == "Cephalosporin Sterile Powder For Injection" || drugLicense == "Betalactum" || drugLicense == "General Sterile Powder For Injection" || dsgcode == "DSG001" || dsgcode == "DSG006" || drugLicense == "Cytotoxic Sterile Powder For Injection" || drugLicense == "Hormones Sterile Powder For Injection")
            {
                //con.Open();
                using (SqlCommand cmd = new SqlCommand("formulationdsgLicense", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@gen_code", gen_code);
                    SqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    reader.Close();
                }      
            }
            else
            {
                //con.Open();
                using (SqlCommand cmd = new SqlCommand("formulationdsgcode", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@gen_code", gen_code);
                    SqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    reader.Close();
                }

               
            }
            dt=GetlatestRate(dt);

            return dt;
        }

        public DataTable GetlatestRate(DataTable dt)
        {
            //Connection();
            //con.Open();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataTable dtp = new DataTable();
                DataTable dts = new DataTable();
                using (SqlCommand cmd = new SqlCommand("LatestRatePO", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@item", dt.Rows[i][0]);
                    SqlDataReader reader = cmd.ExecuteReader();
                   
                    dtp.Load(reader);
                    reader.Close();
                }


                using (SqlCommand cmd = new SqlCommand("LatestRateSupplier", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@item", dt.Rows[i][0]);
                    SqlDataReader reader = cmd.ExecuteReader();

                    dts.Load(reader);
                    reader.Close();
                }
                if (dts.Rows.Count > 0)
                {
                    if (dts.Rows[0]["Inr_combo"].ToString() == "USD")
                    {
                        dts.Rows[0]["Conv_Rate"] = 84;
                    }
                }

                if (dts.Rows.Count > 0 && dtp.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dts.Rows[0][0].ToString()))
                    {
                        DateTime dtsDate;
                        if (DateTime.TryParse(dts.Rows[0][0].ToString(), out dtsDate))
                        {
                            DateTime dtpDate = (DateTime)dtp.Rows[0][0]; // Assuming dtp.Rows[0][0] is already a DateTime
                            if (dtsDate > dtpDate)
                            {
                                dt = fillSupplierRate(dt, dts, i);
                            }
                            else
                            {
                                dt = fillPoRate(dt, dtp, i);
                            }
                        }

                            //if (Convert.ToDateTime(dts.Rows[0][0]) > Convert.ToDateTime(dtp.Rows[0][0]))

                     }
                    else
                    {
                        dt = fillPoRate(dt, dtp, i);
                    }
                }
                else if (dts.Rows.Count > 0)
                {
                    dt = fillSupplierRate(dt, dts, i);
                }
                else if (dtp.Rows.Count > 0)
                {
                    dt = fillPoRate(dt, dtp, i);
                }
            }

            string genname = "";
            if (dt.Rows.Count > 0)
            {
                genname = dt.Rows[0][16].ToString();
            }
            else
            {
                return dt;
            }

            dt=Fill_totalformulation(dt,ref _totalFormulation, genname);

            return dt;
        }

        private DataTable Fill_totalformulation(DataTable dt,ref decimal totalFormulation, string genname)
        {
            try
            {
                //Connection();
                //con.Open();
                DataTable dtdq = new DataTable();
                using (SqlCommand cmd = new SqlCommand("findOQDQ", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@work_order", work_order);
                    SqlDataReader reader = cmd.ExecuteReader();

                    dtdq.Load(reader);
                    reader.Close();
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    dt = ChangeRateFromOtherToKg(dt);
                    dt = changeRateFromUSDtoINR(dt);
                    dt = ChangeQuantityFromOtherToKg(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        if (!row.IsNull("Rate after Conv") && !row.IsNull(8))
                        {
                            row["Rate*Quantity"] = Convert.ToDecimal(row["Rate after Conv"]) * Convert.ToDecimal(row[8]);
                        }
                        row["CustomDuty"] = Math.Round(Convert.ToDecimal(row["CustomDuty"]) * Convert.ToDecimal(row[8]), 2);
                        row["CessOnDuty"] = Math.Round(Convert.ToDecimal(row["CessOnDuty"]) * Convert.ToDecimal(row[8]), 2);
                        row["GST"] = Math.Round(Convert.ToDecimal(row["GST"]) * Convert.ToDecimal(row[8]), 2);
                        row["TransitFreight"] = Math.Round(Convert.ToDecimal(row["TransitFreight"]) * Convert.ToDecimal(row[8]), 2);
                        row["Freight"] = Math.Round(Convert.ToDecimal(row["Freight"]) * Convert.ToDecimal(row[8]), 2);
                        row["Item Total"] = Convert.ToDecimal((Convert.ToDecimal(row["Rate*Quantity"]) +
                    Convert.ToDecimal(row["CustomDuty"]) +
                    Convert.ToDecimal(row["CessOnDuty"]) +
                    Convert.ToDecimal(row["GST"]) +
                    Convert.ToDecimal(row["TransitFreight"]) +
                    Convert.ToDecimal(row["Freight"])).ToString("##,##0.00"));

                    }

                    decimal itemtotalcolumn = Convert.ToDecimal(dt.Compute("SUM([Item Total])", ""));
                    totalFormulation += itemtotalcolumn;

                    DataRow MyNewRow = dt.NewRow();
                    MyNewRow[0] = "";
                    MyNewRow[1] = genname;
                    MyNewRow[3] = 0.0m;
                    MyNewRow[4] = "";
                    MyNewRow[5] = 0.0m;
                    MyNewRow[8] = 0.0m;
                    MyNewRow[9] = dt.Compute("SUM([Item Total])", "");
                    MyNewRow[10] = "";
                    MyNewRow[11] = "";
                    MyNewRow[12] = "";
                    MyNewRow[13] = "";
                    MyNewRow[14] = "";
                    MyNewRow[15] = "";
                    MyNewRow[16] = "";
                    MyNewRow[17] = "";
                    MyNewRow[18] = "";
                    MyNewRow[19] = "";
                    MyNewRow[20] = "";
                    MyNewRow[21] = "";
                    MyNewRow[22] = "";
                    MyNewRow[23] = "";
                    MyNewRow[24] = 0;
                    MyNewRow[25] = 0.0m;
                    MyNewRow[26] = 0.0m;
                    MyNewRow[27] = 0.0m;
                    MyNewRow[28] = 0.0m;
                    MyNewRow[29] = 0.0m;
                    MyNewRow[30] = 0.0m;
                    MyNewRow[31] = 0.0m;
                    MyNewRow[32] = 0.0m;


                    dt.Rows.Add(MyNewRow);
                    dt.AcceptChanges();

                    if (dsg_code == "DSG012") // dry syrup
                    {
                        DataRow MyNewRow1 = dt.NewRow();
                        MyNewRow1[0] = "";
                        MyNewRow1[1] = "";
                        MyNewRow1[3] = 0.0m;
                        MyNewRow1[4] = "";
                        MyNewRow1[5] = 0.0m;
                        MyNewRow1[9] = 0.0m;
                        MyNewRow1[10] = "";
                        MyNewRow1[11] = "";
                        MyNewRow1[12] = "";
                        MyNewRow1[13] = "";
                        MyNewRow1[14] = "";
                        MyNewRow1[15] = "";
                        MyNewRow1[16] = "";
                        MyNewRow1[17] = "";
                        MyNewRow1[18] = "";
                        MyNewRow1[19] = "";
                        MyNewRow1[20] = "";
                        MyNewRow1[21] = "";
                        MyNewRow1[22] = "";
                        MyNewRow1[23] = "";
                        MyNewRow1[24] = 0;
                        MyNewRow1[25] = 0.0m;
                        MyNewRow1[26] = 0.0m;
                        MyNewRow1[27] = 0.0m;
                        MyNewRow1[28] = 0.0m;
                        MyNewRow1[29] = 0.0m;
                        MyNewRow1[30] = 0.0m;
                        MyNewRow1[31] = 0.0m;
                        MyNewRow1[32] = 0.0m;

                        MyNewRow1[2] = totalFormulation / Convert.ToDecimal(Field7txtTotNoUnitInBatch);
                        MyNewRow1[8] = (totalFormulation / Convert.ToDecimal(Field7txtTotNoUnitInBatch)) * (Convert.ToDecimal(dtdq.Rows[0][2]) / Convert.ToDecimal(dtdq.Rows[0][1]));
                        dt.Rows.Add(MyNewRow1);
                        dt.AcceptChanges();
                    }
                    else if (dsg_code == "DSG002") // injection
                    {
                        if (drugLicense == "Betalactum Sterile Powder For Injection" || drugLicense == "Cephalosporin Sterile Powder For Injection" || drugLicense == "General Sterile Powder For Injection" || drugLicense == "Cytotoxic Sterile Powder For Injection" || drugLicense == "Hormones Sterile Powder For Injection")
                        {
                            DataRow firstRow = dt.Rows[0];
                            string Total_no_of_Units = firstRow["TotNoUnit"].ToString();
                            string[] parts = Total_no_of_Units.Split("PCS");
                            int totNoUnit = int.Parse(parts[0]);

                            DataRow MyNewRow1 = dt.NewRow();
                            MyNewRow1[0] = "";
                            MyNewRow1[1] = "";
                            MyNewRow1[2] = "";
                            MyNewRow1[3] = 0.0m;
                            MyNewRow1[4] = "";
                            MyNewRow1[9] = 0.0m;
                            MyNewRow1[10] = "";
                            MyNewRow1[11] = "";
                            MyNewRow1[12] = "";
                            MyNewRow1[13] = "";
                            MyNewRow1[14] = "";
                            MyNewRow1[15] = "";
                            MyNewRow1[16] = "";
                            MyNewRow1[17] = "";
                            MyNewRow1[18] = "";
                            MyNewRow1[19] = "";
                            MyNewRow1[20] = "";
                            MyNewRow1[21] = "";
                            MyNewRow1[22] = "";
                            MyNewRow1[23] = "";
                            MyNewRow1[24] = 0;
                            MyNewRow1[25] = 0.0m;
                            MyNewRow1[26] = 0.0m;
                            MyNewRow1[27] = 0.0m;
                            MyNewRow1[28] = 0.0m;
                            MyNewRow1[29] = 0.0m;
                            MyNewRow1[30] = 0.0m;
                            MyNewRow1[31] = 0.0m;
                            MyNewRow1[32] = 0.0m;


                            MyNewRow1[5] = (totalFormulation / totNoUnit) * Convert.ToDecimal(dtdq.Rows[0][2]);
                            MyNewRow1[8] = (totalFormulation / totNoUnit) * (Convert.ToDecimal(dtdq.Rows[0][2]) / Convert.ToDecimal(dtdq.Rows[0][1]));
                            dt.Rows.Add(MyNewRow1);
                            dt.AcceptChanges();
                        }
                        else
                        {
                            DataRow MyNewRow1 = dt.NewRow();

                            MyNewRow1[0] = "";
                            MyNewRow1[3] = 0.0m;
                            MyNewRow1[5] = 0.0m;
                            MyNewRow1[9] = 0.0m;
                            MyNewRow1[10] = "";
                            MyNewRow1[11] = "";
                            MyNewRow1[12] = "";
                            MyNewRow1[13] = "";
                            MyNewRow1[14] = "";
                            MyNewRow1[15] = "";
                            MyNewRow1[16] = "";
                            MyNewRow1[17] = "";
                            MyNewRow1[18] = "";
                            MyNewRow1[19] = "";
                            MyNewRow1[20] = "";
                            MyNewRow1[21] = "";
                            MyNewRow1[22] = "";
                            MyNewRow1[23] = "";
                            MyNewRow1[24] = 0;
                            MyNewRow1[25] = 0.0m;
                            MyNewRow1[26] = 0.0m;
                            MyNewRow1[27] = 0.0m;
                            MyNewRow1[28] = 0.0m;
                            MyNewRow1[29] = 0.0m;
                            MyNewRow1[30] = 0.0m;
                            MyNewRow1[31] = 0.0m;
                            MyNewRow1[32] = 0.0m;

                            MyNewRow1[1] = totalFormulation.ToString() + "/" + Field7txtTotNoUnitInBatch;
                            MyNewRow1[2] = totalFormulation / Convert.ToDecimal(Field7txtTotNoUnitInBatch);
                            MyNewRow1[4] = "Cost Per Injection as per Determined Quantity-";
                            MyNewRow1[8] = (totalFormulation / Convert.ToDecimal(Field7txtTotNoUnitInBatch)) * (Convert.ToDecimal(dtdq.Rows[0][2]) / Convert.ToDecimal(dtdq.Rows[0][1]));
                            dt.Rows.Add(MyNewRow1);
                            dt.AcceptChanges();
                        }
                    }
                    else if (dsg_code == "DSG005" || dsg_code == "DSG003" || dsg_code == "DSG004" || dsg_code == "DSG009" || dsg_code == "DSG010")
                    {
                        DataRow MyNewRow1 = dt.NewRow();


                        MyNewRow1[0] = "";
                        MyNewRow1[1] = "";
                        MyNewRow1[3] = 0.0m;
                        MyNewRow1[4] = "";
                        MyNewRow1[5] = 0.0m;
                        MyNewRow1[9] = 0.0m;
                        MyNewRow1[10] = "";
                        MyNewRow1[11] = "";
                        MyNewRow1[12] = "";
                        MyNewRow1[13] = "";
                        MyNewRow1[14] = "";
                        MyNewRow1[15] = "";
                        MyNewRow1[16] = "";
                        MyNewRow1[17] = "";
                        MyNewRow1[18] = "";
                        MyNewRow1[19] = "";
                        MyNewRow1[20] = "";
                        MyNewRow1[21] = "";
                        MyNewRow1[22] = "";
                        MyNewRow1[23] = "";
                        MyNewRow1[24] = 0;
                        MyNewRow1[25] = 0.0m;
                        MyNewRow1[26] = 0.0m;
                        MyNewRow1[27] = 0.0m;
                        MyNewRow1[28] = 0.0m;
                        MyNewRow1[29] = 0.0m;
                        MyNewRow1[30] = 0.0m;
                        MyNewRow1[31] = 0.0m;
                        MyNewRow1[32] = 0.0m;

                        MyNewRow1[2] = totalFormulation / Convert.ToDecimal(Field7txtTotNoUnitInBatch);
                        MyNewRow1[8] = (totalFormulation / Convert.ToDecimal(Field7txtTotNoUnitInBatch)) * (Convert.ToDecimal(dtdq.Rows[0][2]) / Convert.ToDecimal(dtdq.Rows[0][1]));
                        dt.Rows.Add(MyNewRow1);
                        dt.AcceptChanges();
                    }
                    else if (dsg_code == "DSG006")
                    {
                        DataRow MyNewRow1 = dt.NewRow();

                        MyNewRow1[0] = "";
                        MyNewRow1[1] = "";
                        MyNewRow1[2] = "";
                        MyNewRow1[3] = 0.0m;
                        MyNewRow1[4] = "";
                        MyNewRow1[9] = 0.0m;
                        MyNewRow1[10] = "";
                        MyNewRow1[11] = "";
                        MyNewRow1[12] = "";
                        MyNewRow1[13] = "";
                        MyNewRow1[14] = "";
                        MyNewRow1[15] = "";
                        MyNewRow1[16] = "";
                        MyNewRow1[17] = "";
                        MyNewRow1[18] = "";
                        MyNewRow1[19] = "";
                        MyNewRow1[20] = "";
                        MyNewRow1[21] = "";
                        MyNewRow1[22] = "";
                        MyNewRow1[23] = "";
                        MyNewRow1[24] = 0;
                        MyNewRow1[25] = 0.0m;
                        MyNewRow1[26] = 0.0m;
                        MyNewRow1[27] = 0.0m;
                        MyNewRow1[28] = 0.0m;
                        MyNewRow1[29] = 0.0m;
                        MyNewRow1[30] = 0.0m;
                        MyNewRow1[31] = 0.0m;
                        MyNewRow1[32] = 0.0m;

                        MyNewRow1[5] = (totalFormulation / 100000) * Convert.ToDecimal(dtdq.Rows[0][2]);
                        MyNewRow1[8] = (totalFormulation / 100000) * (Convert.ToDecimal(dtdq.Rows[0][2]) / Convert.ToDecimal(dtdq.Rows[0][1]));
                        dt.Rows.Add(MyNewRow1);
                        dt.AcceptChanges();
                    }
                    else
                    {
                        DataRow MyNewRow1 = dt.NewRow();

                        MyNewRow1[0] = "";
                        MyNewRow1[1] = "";
                        MyNewRow1[2] = "";
                        MyNewRow1[3] = 0.0m;
                        MyNewRow1[4] = "";
                        MyNewRow1[9] = 0.0m;
                        MyNewRow1[10] = "";
                        MyNewRow1[11] = "";
                        MyNewRow1[12] = "";
                        MyNewRow1[13] = "";
                        MyNewRow1[14] = "";
                        MyNewRow1[15] = "";
                        MyNewRow1[16] = "";
                        MyNewRow1[17] = "";
                        MyNewRow1[18] = "";
                        MyNewRow1[19] = "";
                        MyNewRow1[20] = "";
                        MyNewRow1[21] = "";
                        MyNewRow1[22] = "";
                        MyNewRow1[23] = "";
                        MyNewRow1[24] = 0;
                        MyNewRow1[25] = 0.0m;
                        MyNewRow1[26] = 0.0m;
                        MyNewRow1[27] = 0.0m;
                        MyNewRow1[28] = 0.0m;
                        MyNewRow1[29] = 0.0m;
                        MyNewRow1[30] = 0.0m;
                        MyNewRow1[31] = 0.0m;
                        MyNewRow1[32] = 0.0m;

                        MyNewRow1[5] = (totalFormulation / 100000) * Convert.ToDecimal(dtdq.Rows[0][2]);
                        MyNewRow1[8] = (totalFormulation / 100000) * (Convert.ToDecimal(dtdq.Rows[0][2]) / Convert.ToDecimal(dtdq.Rows[0][1]));
                        dt.Rows.Add(MyNewRow1);
                        dt.AcceptChanges();
                    }

                    if (ListOfGenCodes.Count > 0)  // this means combi pack costing
                    {
                        dt = CalculateFormulationCostForCombi(itemtotalcolumn, dt);  // this function is called to calculate formulation rate per Wo for each item in list of gencodes in combi pack
                    }

                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }

        public DataTable ChangeRateFromOtherToKg(DataTable dt)
        {
            for (int index = 0; index < dt.Rows.Count; index++)
            {
                //Connection();
                object conv_from_master = null;
                object eq_from_master = null;

                if (dt.Rows[index][4].ToString().ToUpper() == "BOU" || dt.Rows[index][4].ToString().ToUpper() == "MIU" || dt.Rows[index][4].ToString().ToUpper() == "LTR")
                {
                    if (dt.Rows[index][14].ToString() == "API")
                    {
                        //con.Open();

                        using (SqlCommand cmd = new SqlCommand("APItoKG", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@code", dt.Rows[index][0]);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                conv_from_master = reader[0];
                                eq_from_master = reader[2];
                            }
                            reader.Close();
                        }
                    }
                    else if (dt.Rows[index][14].ToString() == "Excipient")
                    {
                        //con.Open();
                        using (SqlCommand cmd = new SqlCommand("ExptoKG", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@code", dt.Rows[index][0]);

                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                conv_from_master = reader[0];
                                eq_from_master = reader[2];
                            }
                            reader.Close();

                        }
                    }
                    else if (dt.Rows[index][14].ToString() == "Coating")
                    {
                        conv_from_master = 1;
                        eq_from_master = 1;
                    }
                }
                if (dt.Rows[index][4].ToString().ToUpper() == "BOU")
                {
                    double rte = 0;

                    if (dt.Rows[index][7].ToString().ToUpper() == "KG")
                        rte = (double.Parse(dt.Rows[index][3].ToString()) * double.Parse(conv_from_master.ToString()));
                    else if (dt.Rows[index][7].ToString().ToUpper() == "GM")
                        rte = (double.Parse(dt.Rows[index][3].ToString()) * double.Parse(conv_from_master.ToString()) / 1000);
                    else if (dt.Rows[index][7].ToString().ToUpper() == "MG")
                        rte = (double.Parse(dt.Rows[index][3].ToString()) * double.Parse(conv_from_master.ToString()) / 1000000);

                    dt.Rows[index][5] = Math.Round(rte, 5);
                }
                else if (dt.Rows[index][4].ToString().ToUpper() == "MIU")
                {
                    double rte = (double.Parse(dt.Rows[index][3].ToString()) * double.Parse(conv_from_master.ToString()));
                    dt.Rows[index][5] = Math.Round(rte, 5);
                }
                else if (dt.Rows[index][4].ToString().ToUpper() == "LTR")
                {
                    double rte = (double.Parse(dt.Rows[index][3].ToString()) / double.Parse(eq_from_master.ToString()));
                    dt.Rows[index][5] = Math.Round(rte, 5);
                }
                else if (dt.Rows[index][4].ToString().ToUpper() == "GM")
                {
                    double rte = (double.Parse(dt.Rows[index][3].ToString()) * 1000);
                    dt.Rows[index][5] = Math.Round(rte, 5);
                }
                else if (dt.Rows[index][4].ToString().ToUpper() == "MG")
                {
                    double rte = (double.Parse(dt.Rows[index][3].ToString()) * 100000);
                    dt.Rows[index][5] = Math.Round(rte, 5);
                }
                else if (dt.Rows[index][4].ToString().ToUpper() == "KG")
                {
                    double rte = double.Parse(dt.Rows[index][3].ToString());
                    dt.Rows[index][5] = Math.Round(rte, 5);
                }
            }

            return dt;
        }

        public DataTable changeRateFromUSDtoINR(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                row["Rate after Conv"] = Math.Round(Convert.ToDecimal(row[5]) * Convert.ToDecimal(row["Conv_rate"]), 4);
            }
            return dt;
        }

        public DataTable fillSupplierRate(DataTable dt, DataTable dts, int i)
        {
            dt.Rows[i][3] = dts.Rows[0]["price"];
            dt.Rows[i][4] = dts.Rows[0]["unit"];
            dt.Rows[i][10] = "Supplier Master";
            dt.Rows[i][12] = dts.Rows[0]["date_of_quotation"];
            dt.Rows[i]["Currency"] = dts.Rows[0]["Inr_combo"];
            dt.Rows[i]["Conv_rate"] = dts.Rows[0]["Conv_Rate"];
            return dt;
        }
        public DataTable fillPoRate(DataTable dt, DataTable dtp, int i)
        {
            DataColumn uomColumn = dt.Columns["UOM for Rate"];
            uomColumn.MaxLength = 50;

            DataColumn status = dt.Columns["Status"];
            status.MaxLength = 50;

            DataColumn currency = dt.Columns["Currency"];
            currency.MaxLength = 50;

            DataColumn date = dt.Columns["PO Date/Quotation Date"];
            date.MaxLength = 50;
            //DataColumn rate = dt.Columns["Conv_rate"];
            //rate.MaxLength = 50;
            //DataColumn duty = dt.Columns["CustomDuty"];
            //duty.MaxLength = 50;
            //DataColumn cess = dt.Columns["CessOnDuty"];
            //cess.MaxLength = 50;
            //DataColumn gst = dt.Columns["GST"];
            //gst.MaxLength = 50;
            //DataColumn Tfreight = dt.Columns["TransitFreight"];
            //Tfreight.MaxLength = 50;
            //DataColumn freight = dt.Columns["Freight"];
            //freight.MaxLength = 50;
            DataColumn billentryno = dt.Columns["billentryno"];
            billentryno.MaxLength = 50;

            try
            {
                //Connection();
                //con.Open();
                DataTable dtboe = new DataTable();
                using (SqlCommand cmd = new SqlCommand("FillPORate", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PoNumber", dtp.Rows[0]["order_no"]);
                    cmd.Parameters.AddWithValue("@itemcode", dt.Rows[i][0]);
                    SqlDataReader reader = cmd.ExecuteReader();

                    dtboe.Load(reader);
                    reader.Close();
                }

                if (dtboe.Rows.Count > 0 && !Convert.IsDBNull(dtboe.Rows[0]["quantity"]) && Convert.ToDecimal(dtboe.Rows[0]["quantity"]) != 0)
                {
                    dtboe = ChangeQuantityFromOtherToKg(dtboe);
                    dt.Rows[i][3] = dtboe.Rows[0]["ExporterPrice"];
                    dt.Rows[i][4] = dtboe.Rows[0]["uom"];
                    dt.Rows[i][10] = dtboe.Rows[0]["PoNumber"];
                    
                    dt.Rows[i][12] = dtboe.Rows[0]["BillEntryDate"];
                    dt.Rows[i]["Currency"] = dtboe.Rows[0]["CurrencyConvUnit"];
                    dt.Rows[i]["Conv_rate"] = dtboe.Rows[0]["CurrencyConvINR"];
                    dt.Rows[i]["CustomDuty"] = Math.Round(Convert.ToDecimal(dtboe.Rows[0]["CustomDuty"]) / Convert.ToDecimal(dtboe.Rows[0]["quantity"]), 4);
                    dt.Rows[i]["CessOnDuty"] = Math.Round(Convert.ToDecimal(dtboe.Rows[0]["CessOnDuty"]) / Convert.ToDecimal(dtboe.Rows[0]["quantity"]), 4);
                    dt.Rows[i]["GST"] = Math.Round(Convert.ToDecimal(dtboe.Rows[0]["GST"]) / Convert.ToDecimal(dtboe.Rows[0]["quantity"]), 4);
                    dt.Rows[i]["TransitFreight"] = Math.Round(Convert.ToDecimal(dtboe.Rows[0]["TransitFreight"]) / Convert.ToDecimal(dtboe.Rows[0]["quantity"]), 4);
                    dt.Rows[i]["Freight"] = Math.Round(Convert.ToDecimal(dtboe.Rows[0]["Freight"]) / Convert.ToDecimal(dtboe.Rows[0]["quantity"]), 4);
                    dt.Rows[i]["billentryno"] = dtboe.Rows[0]["billentryno"];
                }
                else
                {
                    dt.Rows[i][3] = dtp.Rows[0]["rate"];
                    dt.Rows[i][4] = dtp.Rows[0]["unit"];
                    dt.Rows[i][10] = dtp.Rows[0]["order_no"];
                   
                    dt.Rows[i][12] = dtp.Rows[0]["order_date"];
                    dt.Rows[i]["Currency"] = dtp.Rows[0]["currency"];
                    dt.Rows[i]["Conv_rate"] = dtp.Rows[0]["Conv_Rate"];
                }
                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }


        public DataTable ChangeQuantityFromOtherToKg(DataTable dt)
        {
            for (int index = 0; index < dt.Rows.Count; index++)
            {
                //Connection();
                object conv_from_master = null;
                object eq_from_master = null;
                if (dt.Rows[index][7].ToString().ToUpper() == "BOU" || dt.Rows[index][4].ToString().ToUpper() == "MIU" || dt.Rows[index][4].ToString().ToUpper() == "LTR")
                {
                    if (dt.Rows[index][14].ToString() == "API" || dt.Rows[index][14].ToString() == "A.P.I.")
                    {
                        
                        //con.Open();
                       
                        using (SqlCommand cmd = new SqlCommand("APItoKG", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@code", dt.Rows[index][0]);
                          
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                conv_from_master = reader[0];
                                eq_from_master = reader[2];
                            }
                            reader.Close();

                        }
                    }
                    else if (dt.Rows[index][14].ToString() == "Excipient")
                    {
                        //con.Open();
                        using (SqlCommand cmd = new SqlCommand("ExptoKG", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@code", dt.Rows[index][0]);

                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                conv_from_master = reader[0];
                                eq_from_master = reader[2];
                            }
                            reader.Close();

                        }
                    }
                }
                if (dt.Rows[index][7].ToString().ToUpper() == "BOU")
                {
                    double qty = (Convert.ToDouble(dt.Rows[index][6]) * 1000 / Convert.ToDouble(conv_from_master));
                    dt.Rows[index][8] = Math.Round(qty, 5);
                }
                else if (dt.Rows[index][7].ToString().ToUpper() == "MIU")
                {
                    double qty = (Convert.ToDouble(dt.Rows[index][6]) / Convert.ToDouble(conv_from_master));
                    dt.Rows[index][8] = Math.Round(qty, 5);
                }
                else if (dt.Rows[index][7].ToString().ToUpper() == "LTR")
                {
                    double qty = (Convert.ToDouble(dt.Rows[index][6]) * Convert.ToDouble(eq_from_master));
                    dt.Rows[index][8] = Math.Round(qty, 5);
                }
                else if (dt.Rows[index][7].ToString().ToUpper() == "GM")
                {
                    double qty = (Convert.ToDouble(dt.Rows[index][6]) / 1000);
                    dt.Rows[index][8] = Math.Round(qty, 5);
                }
                else if (dt.Rows[index][7].ToString().ToUpper() == "MG")
                {
                    double qty = (Convert.ToDouble(dt.Rows[index][6]) / 100000);
                    dt.Rows[index][8] = Math.Round(qty, 5);
                }
                else if (dt.Rows[index][7].ToString().ToUpper() == "KG")
                {
                    double qty = (Convert.ToDouble(dt.Rows[index][6]));
                    dt.Rows[index][8] = Math.Round(qty, 5);
                }
            }
            return dt;
        }


        public DataTable CalculateFormulationCostForCombi(decimal ItemTotal, DataTable dtc)
        {
          
            decimal fpd = 0.0m; // formulation rate per determined qty
            decimal fpo = 0.0m; // formulation rate per ordered qty
                                // ----------------------------------------------calculate cost
            //Connection();
            //con.Open();
            DataTable dts = new DataTable();
            DataTable dtf = new DataTable();
            using (SqlCommand cmd = new SqlCommand("OQDQ", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@work_order", wono);
                SqlDataReader reader = cmd.ExecuteReader();
                dts.Load(reader);
                reader.Close();
            }
            if (dts.Rows.Count == 0)
            {

                using (SqlCommand cmd = new SqlCommand("OQDQifnotfind", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@work_order", wono);
                    SqlDataReader reader = cmd.ExecuteReader();
                    dts.Load(reader);
                    reader.Close();
                }
                if (dts.Rows.Count > 0)
                {
                    decimal determinedqty = Math.Round(Convert.ToDecimal(dts.Rows[0][1]) * 1.05m, 2); // ordered qty plus 5 % of ordered qty
                    dts.Rows[0][2] = determinedqty;
                }
            }

            using (SqlCommand cmd = new SqlCommand("formulationdetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GENCODE", GENCODE);
                SqlDataReader reader = cmd.ExecuteReader();
                dtf.Load(reader);
                reader.Close();
            }
                if (dts != null && dts.Rows.Count > 0)
                {
                    decimal ColFormulationTotal1 = ItemTotal;
                    decimal ColSize2 = Convert.ToDecimal(dts.Rows[0]["Size"]);
                    decimal ColLotSize3;
                    decimal ColDetermQty4 = Convert.ToDecimal(dts.Rows[0]["Determined Quantity"]);
                    decimal ColOrderedQty5 = Convert.ToDecimal(dts.Rows[0]["Ordered Quantity"]);

                    if (dtf.Rows[0]["tot_no_of_unit"] == DBNull.Value || Convert.ToDecimal(dtf.Rows[0]["tot_no_of_unit"]) == 0)
                    {
                        dtf.Rows[0]["tot_no_of_unit"] = 1;
                    }

                    if (dtf.Rows[0]["unit"].Equals("LTR") || dtf.Rows[0]["unit"].ToString().ToUpper().Equals("KG")) // If UOM is LTR or KG
                    {
                        ColLotSize3 = Convert.ToDecimal(dtf.Rows[0]["lot_size"]) * 1000;
                        decimal ColFormulationRatePerUnit6 = (ColFormulationTotal1 / ColLotSize3 * ColSize2) * (ColDetermQty4 / ColOrderedQty5);
                        fpo = Math.Round(Convert.ToDecimal(ColFormulationRatePerUnit6), 6); // Formulation per Unit per workOrder
                    }
                    else
                    {
                        ColLotSize3 = Convert.ToDecimal(dtf.Rows[0]["lot_size"]); // If UOM is PCS
                        decimal ColFormulationRatePerUnit6 = (ColFormulationTotal1 / ColLotSize3 * ColSize2) * (ColDetermQty4 / ColOrderedQty5);
                        fpo = Math.Round(Convert.ToDecimal(ColFormulationRatePerUnit6), 6);
                    }

                    //SqlHelper objsqlhelper = new SqlHelper();
                    //string drugLicense, dsgcode;
                    //DataTable dtbl = objsqlhelper.GetDataTableProduction(string.Format("select Drug_Licence,drug_dosg_form from finished_product where gen_code ='{0}'", GENCODE));
                    //if (dtbl.Rows.Count > 0)
                    //{
                    //    drugLicense = dtbl.Rows[0][0].ToString();
                    //    dsgcode = dtbl.Rows[0][1].ToString();
                    //}

                    if (drugLicense == "Betalactum Sterile Powder For Injection" || drugLicense == "Cephalosporin Sterile Powder For Injection" || drugLicense == "Betalactum" || drugLicense == "General Sterile Powder For Injection" || dsgcode == "DSG001" || dsgcode == "DSG006" || drugLicense == "Cytotoxic Sterile Powder For Injection" || drugLicense == "Hormones Sterile Powder For Injection")
                    {
                        if (!string.IsNullOrEmpty(dtf.Rows[0]["lot_size"].ToString()) && !string.IsNullOrEmpty(dts.Rows[0]["Size"].ToString()) && Convert.ToDecimal(dtf.Rows[0]["lot_size"]) != 0 && Convert.ToDecimal(dts.Rows[0]["Size"]) != 0)
                        {
                            fpd = Math.Round(Convert.ToDecimal(ColFormulationTotal1 / Convert.ToDecimal(dtf.Rows[0]["lot_size"]) * Convert.ToDecimal(dts.Rows[0]["Size"])), 4);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtf.Rows[0]["tot_no_of_unit"].ToString()) && !string.IsNullOrEmpty(dts.Rows[0]["Size"].ToString()) && Convert.ToDecimal(dtf.Rows[0]["tot_no_of_unit"]) != 0 && Convert.ToDecimal(dts.Rows[0]["Size"]) != 0)
                        {
                            fpd = Math.Round(Convert.ToDecimal(ColFormulationTotal1) / Convert.ToDecimal(dtf.Rows[0]["tot_no_of_unit"]) * Convert.ToDecimal(dts.Rows[0]["Size"]), 4); // Formulation Total / Determined qty
                        }
                    }

                    decimal PowderWeightPerUnit = Convert.ToDecimal(dtf.Rows[0]["unit_size"]) * (Convert.ToDecimal(dts.Rows[0]["size"]) / Convert.ToDecimal(dtf.Rows[0]["per"]));
                    decimal TotNoUnitInBatch = Math.Round(Convert.ToDecimal(dtf.Rows[0]["bottle_size"]) * 1000 / PowderWeightPerUnit, 0);

                    switch (dsgcode)
                    {
                        case "DSG012":
                        fpd = Math.Round(Convert.ToDecimal(Convert.ToDecimal(ColFormulationTotal1) / TotNoUnitInBatch), 4);

                        fpo = Math.Round(Convert.ToDecimal(fpd * Convert.ToDecimal(ColDetermQty4) / Convert.ToDecimal(ColOrderedQty5)), 4);
                            break;
                        case "DSG002":
                            if (drugLicense == "Betalactum Sterile Powder For Injection" || drugLicense == "Cephalosporin Sterile Powder For Injection" || drugLicense == "Betalactum" || drugLicense == "General Sterile Powder For Injection" || dsgcode == "DSG001" || dsgcode == "DSG006" || drugLicense == "Cytotoxic Sterile Powder For Injection" || drugLicense == "Hormones Sterile Powder For Injection")
                            {
                                if (!string.IsNullOrEmpty(dtf.Rows[0]["lot_size"].ToString()) && !string.IsNullOrEmpty(dts.Rows[0]["Size"].ToString()) && Convert.ToDecimal(dtf.Rows[0]["lot_size"]) != 0 && Convert.ToDecimal(dts.Rows[0]["Size"]) != 0)
                                {
                                    decimal ColFormulationRatePerUnit6 = (Convert.ToDecimal(ColFormulationTotal1) / Convert.ToDecimal(dtf.Rows[0]["lot_size"])) * Convert.ToDecimal(ColDetermQty4 / ColOrderedQty5);
                                    fpo = Math.Round(ColFormulationRatePerUnit6, 6);
                                }
                            }
                            else
                            {
                                decimal ColFormulationRatePerUnit6 = (Convert.ToDecimal(ColFormulationTotal1) / Convert.ToDecimal(dtf.Rows[0]["tot_no_of_unit"])) * Convert.ToDecimal(ColDetermQty4 / ColOrderedQty5);
                                fpo = Math.Round(ColFormulationRatePerUnit6, 6);
                            }
                            break;
                        default:
                            break;
                    }

                    decimal fpo_perpack = fpo * Convert.ToDecimal(dts.Rows[0]["qty_per_pack"]);
                    decimal fpd_perpack = fpd * Convert.ToDecimal(dts.Rows[0]["qty_per_pack"]);

                    TotalFormulationRate_perpack_perWo = TotalFormulationRate_perpack_perWo + Convert.ToDouble(fpo_perpack);
                    TotalFormulationRate_perpack_perD = TotalFormulationRate_perpack_perD + Convert.ToDouble(fpd_perpack);

                    string strQty = "Ordered Qty: " + dts.Rows[0]["Ordered Quantity"].ToString() + " Determined Qty:  " + dts.Rows[0]["Determined Quantity"].ToString() + " Qty Per Pack:" + dts.Rows[0]["qty_per_pack"].ToString();
                    DataRow MyNewRow2;
                    MyNewRow2 = dtc.NewRow();
                    MyNewRow2[0] = "♦";
                    MyNewRow2[1] = strQty;
                    dtc.Rows.Add(MyNewRow2);
                    dtc.AcceptChanges();

                    string strDescription = "Formulation Rate Per Unit /WO: " + fpo.ToString() + " Formulation Rate Per Unit/Determined Qty:  " + fpd.ToString();
                    DataRow MyNewRow;
                    MyNewRow = dtc.NewRow();
                    MyNewRow[0] = "♦";
                    MyNewRow[1] = strDescription;
                    dtc.Rows.Add(MyNewRow);
                    dtc.AcceptChanges();

                    string strpackprice = "Formulation Rate Per Per pack /WO: " + fpo_perpack.ToString() + "  Formulation Rate Per Pack /Determined Qty:  " + fpd_perpack.ToString();
                    DataRow MyNewRow3;
                    MyNewRow3 = dtc.NewRow();
                    MyNewRow3[0] = "♦";
                    MyNewRow3[1] = strpackprice;
                    dtc.Rows.Add(MyNewRow3);
                    dtc.AcceptChanges();
                }
            return dtc;

         
        }


        private void FillTotals(DataTable dt = null, string msg = null)
        {
          

            if (dt != null && dt.Rows.Count > 0 && msg == "NonCombi")
            {
                costing.FormulationCosting = dt.Rows[0][7] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0][7]) : 0;
                 
                costing.ComponentCosting  = dt.Rows[0][6]!=DBNull.Value ? Convert.ToDecimal(dt.Rows[0][6]) : 0;
               costing.TotalCosting = dt.Rows[0][5]!=DBNull.Value ? Convert.ToDecimal(dt.Rows[0][5]) : 0;
                //txtCostPerUnit.Text = IsNumeric(dt.Rows[0][4]) ? dt.Rows[0][4].ToString() : "0";
                //txtWOQty.Text = dt.Rows[0]["Ordered Quantity"].ToString();
                //lblFinalTotal.Text = "Previous Price Per Unit";
                //txtPartyName.Text = txt_PartyName.Text;
            }
            else if (dt != null && dt.Rows.Count > 0 && msg == "combi")
            {
                costing.FormulationCosting = dt.Rows[0][6]!=DBNull.Value ?Convert.ToDecimal(dt.Rows[0][6]) : 0;
                costing.ComponentCosting = dt.Rows[0][5] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0][5]) : 0;
                costing.TotalCosting = dt.Rows[0][4] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0][4]) : 0;
                //txtCostPerUnit.Text = IsNumeric(dt.Rows[0][3]) ? dt.Rows[0][3].ToString() : "0";
                //lblFinalTotal.Text = "Previous Price Per Pack";
                //txtWOQty.Text = dt.Rows[0][1].ToString();
                //txtPartyName.Text = txt_PartyName.Text;
            }
           
            //if (sb.ToString() != null)
            //{
            //    txtAPICalculationPrice.Text = sb.ToString().TrimEnd(',');
            //}
        }
        #endregion





















       



























    }

}
