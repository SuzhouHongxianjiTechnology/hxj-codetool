using NPOI.XSSF.UserModel;
using NPOI.XSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.Util;
using System.IO;
using System.Data;
using NPOI.HSSF.UserModel;
using System.Globalization;
using System.ComponentModel;
using NPOI.SS.Util;

namespace Albert.Utilities
{
    //目前仅支持*.xls操作类
    public class NpoiExcelHelper
    {
        #region 属性
        private static NpoiExcelHelper npoiExcelExportHelper;
        #endregion

        #region IExcelProvider 成员方法
        //单例
        public static NpoiExcelHelper Instance
        {
            get => npoiExcelExportHelper ?? (npoiExcelExportHelper = new NpoiExcelHelper());
            set => npoiExcelExportHelper = value;
        }

        #region Read
        private static IFont GetFont(IWorkbook workbook, HSSFColor color)
        {
            var font = workbook.CreateFont();
            font.Color = color.Indexed;
            font.FontHeightInPoints = 10;
            font.IsBold = true;
            font.IsItalic = true;
            return font;
        }

        /// <summary>
        /// 从指定路径读取Excel内容
        /// </summary>
        /// <param name="filePath"></param>
        public void ReadFromExcelFile(string filePath)
        {
            IWorkbook wk = null;
            string extension = System.IO.Path.GetExtension(filePath);
            try
            {
                FileStream fs = File.OpenRead(filePath);
                if (extension.Equals(".xls"))
                {
                    //把xls文件中的数据写入wk中
                    wk = new HSSFWorkbook(fs);
                }
                else
                {
                    //把xlsx文件中的数据写入wk中
                    wk = new XSSFWorkbook(fs);
                }

                fs.Close();
                //读取当前表数据
                ISheet sheet = wk.GetSheetAt(0);

                IRow row = sheet.GetRow(0);  //读取当前行数据
                //LastRowNum 是当前表的总行数-1（注意）
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    row = sheet.GetRow(i);  //读取当前行数据
                    if (row != null)
                    {
                        //LastCellNum 是当前行的总列数
                        for (int j = 0; j < row.LastCellNum; j++)
                        {
                            //读取该行的第j列数据
                            string value = row.GetCell(j).ToString();
                            Console.Write(value.ToString() + " ");
                        }
                        Console.WriteLine("\n");
                    }
                }
            }
            catch (Exception e)
            {
                //只在Debug模式下才输出
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///获取cell的数据，并设置为对应的数据类型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public object GetCellValue(ICell cell)
        {
            object value = null;
            try
            {
                if (cell.CellType != CellType.Blank)
                {
                    switch (cell.CellType)
                    {
                        case CellType.Numeric:
                            // Date comes here
                            if (DateUtil.IsCellDateFormatted(cell))
                            {
                                value = cell.DateCellValue;
                            }
                            else
                            {
                                // Numeric type
                                value = cell.NumericCellValue;
                            }
                            break;
                        case CellType.Boolean:
                            // Boolean type
                            value = cell.BooleanCellValue;
                            break;
                        case CellType.Formula:
                            value = cell.CellFormula;
                            break;
                        default:
                            // String type
                            value = cell.StringCellValue;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                value = "";
            }

            return value;
        }
        #endregion

        #region SetOrWrite
        public static void SetCellValues(ICell cell, string cellType, string cellValue)
        {
            switch (cellType)
            {
                case "System.String": //字符串类型
                    double result;
                    if (double.TryParse(cellValue, out result))
                        cell.SetCellValue(result);
                    else
                        cell.SetCellValue(cellValue);
                    break;
                case "System.DateTime": //日期类型
                    DateTime dateV;
                    DateTime.TryParse(cellValue, out dateV);
                    cell.SetCellValue(dateV);
                    break;
                case "System.Boolean": //布尔型
                    bool boolV;
                    bool.TryParse(cellValue, out boolV);
                    cell.SetCellValue(boolV);
                    break;
                case "System.Int16": //整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    int intV;
                    int.TryParse(cellValue, out intV);
                    cell.SetCellValue(intV);
                    break;
                case "System.Decimal": //浮点型
                case "System.Double":
                    double doubV;
                    double.TryParse(cellValue, out doubV);
                    cell.SetCellValue(doubV);
                    break;
                case "System.DBNull": //空值处理
                    cell.SetCellValue("");
                    break;
                default:
                    cell.SetCellValue("");
                    break;
            }
        }

        /// <summary>
        /// 根据数据类型设置不同类型的cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="obj"></param>
        public static void SetCellValue(ICell cell, object obj)
        {
            if (obj.GetType() == typeof(int))
            {
                cell.SetCellValue((int)obj);
            }
            else if (obj.GetType() == typeof(double))
            {
                cell.SetCellValue((double)obj);
            }
            else if (obj.GetType() == typeof(IRichTextString))
            {
                cell.SetCellValue((IRichTextString)obj);
            }
            else if (obj.GetType() == typeof(string))
            {
                cell.SetCellValue(obj.ToString());
            }
            else if (obj.GetType() == typeof(DateTime))
            {
                cell.SetCellValue((DateTime)obj);
            }
            else if (obj.GetType() == typeof(bool))
            {
                cell.SetCellValue((bool)obj);
            }
            else
            {
                cell.SetCellValue(obj.ToString());
            }
        }
        #endregion

        /// <summary>
        /// 从第几行第几列开始写表格
        /// </summary>
        /// <param name="excelFileName"></param>
        /// <param name="dtIn"></param>
        /// <returns></returns>
        public void WriteDT2Excel(string excelFileName, DataTable dtIn,int rowStart=0,int colStart=0)
        {
            if(dtIn == null)
            {
                Console.WriteLine("不要传入空的数据，谢谢");
                return;
            }
            else if (!File.Exists(excelFileName))
            {
                Console.WriteLine("文件不存在");
                return;
            }
            try
            {
                using var fileStream = new FileStream(excelFileName, FileMode.Open, FileAccess.ReadWrite);
                IWorkbook workbook;
                ICellStyle cellStyle;//单元格样式
                IDataFormat dataFormatCustom; //数据显示格式
                ICell cell;//单元格对象
                IRow row; //行对象
                IFont font; //字体，设置字体时需要
                ISheet sheet;

                //表头样式
                workbook = new HSSFWorkbook(fileStream);
                cellStyle = workbook.CreateCellStyle();
                //水平居中、垂直居中
                cellStyle.Alignment = HorizontalAlignment.Center;
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                //设置字体颜色
                //cellStyle.SetFont(GetFont(workbook, new HSSFColor.Orange())); 
                //设置边框
                cellStyle.BorderBottom = BorderStyle.Thin;
                cellStyle.BorderLeft = BorderStyle.Thin;
                cellStyle.BorderRight = BorderStyle.Thin;
                cellStyle.BorderTop = BorderStyle.Thin;
                //自动换行
                cellStyle.WrapText = true;
                //设置字体
                font = workbook.CreateFont();
                font.FontName = "楷体";
                //字体颜色
                font.Color = HSSFColor.Red.Index;
                //字体加粗样式
                font.IsBold = true;
                //样式里的字体设置具体的字体样式
                cellStyle.SetFont(font);
                //设置背景色
                cellStyle.FillForegroundColor = HSSFColor.Yellow.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;
                cellStyle.FillBackgroundColor =HSSFColor.Yellow.Index;
                //设置数据显示格式
                ICellStyle dateStyle = workbook.CreateCellStyle();//样式
                //文字水平对齐方式 垂直对齐方式
                dateStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                dateStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                //设置数据显示格式
                dataFormatCustom = workbook.CreateDataFormat();
                dateStyle.DataFormat = dataFormatCustom.GetFormat("yyyy-MM-dd HH:mm:ss");
                //获取表单第1个表单
                sheet = workbook.GetSheetAt(0);
                for (int i = 0; i < dtIn.Rows.Count; i++)
                {
                    row = sheet.CreateRow(i + rowStart);//创建第i+2行
                    for (int j = 0; j < dtIn.Columns.Count; j++)
                    {
                        cell = row.CreateCell(j + colStart);//创建第j列
                        cell.CellStyle = cellStyle;
                        //根据数据类型设置不同类型的cell
                        SetCellValue(cell, dtIn.Rows[i][j]);
                        //如果是日期，则设置日期显示的格式
                        if (dtIn.Rows[i][j].GetType() == typeof(DateTime))
                        {
                            cell.CellStyle = dateStyle;
                        }
                        //如果要根据内容自动调整列宽，需要先setCellValue再调用
                        sheet.AutoSizeColumn(j + colStart);
                    }
                }

                //合并单元格，如果要合并的单元格中都有数据，只会保留左上角的
                //CellRangeAddress(0, 2, 0, 0)，合并0-2行，0-0列的单元格
                //CellRangeAddress region = new CellRangeAddress(0, 2, 0, 0);
                //sheet.AddMergedRegion(region);
                using var fileStream2 = new FileStream(excelFileName, FileMode.Open, FileAccess.ReadWrite);
                workbook.Write(fileStream2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }           
        }

        #region Npoi之Excel数据导出
        /// <summary>
        /// 先创建行，然后在创建对应的列
        /// 创建Excel中指定的行
        /// </summary>
        /// <param name="sheet">Excel工作表对象</param>
        /// <param name="rowNum">创建第几行(从0开始)</param>
        /// <param name="rowHeight">行高</param>
        public HSSFRow CreateRow(ISheet sheet, int rowNum, float rowHeight)
        {
            HSSFRow row = (HSSFRow)sheet.CreateRow(rowNum); //创建行
            row.HeightInPoints = rowHeight; //设置列头行高
            return row;
        }

        /// <summary>
        /// 创建行内指定的单元格
        /// </summary>
        /// <param name="row">需要创建单元格的行</param>
        /// <param name="cellStyle">单元格样式</param>
        /// <param name="cellNum">创建第几个单元格(从0开始)</param>
        /// <param name="cellValue">给单元格赋值</param>
        /// <returns></returns>
        public HSSFCell CreateCells(HSSFRow row, HSSFCellStyle cellStyle, int cellNum, string cellValue)
        {
            HSSFCell cell = (HSSFCell)row.CreateCell(cellNum); //创建单元格
            cell.CellStyle = cellStyle; //将样式绑定到单元格
            if (!string.IsNullOrWhiteSpace(cellValue))
            {
                //单元格赋值
                cell.SetCellValue(cellValue);
            }

            return cell;
        }

        /// <summary>
        /// 行内单元格常用样式设置
        /// </summary>
        /// <param name="workbook">Excel文件对象</param>
        /// <param name="hAlignment">水平布局方式</param>
        /// <param name="vAlignment">垂直布局方式</param>
        /// <param name="fontHeightInPoints">字体大小</param>
        /// <param name="isAddBorder">是否需要边框</param>
        /// <param name="boldWeight">字体加粗 (None = 0,Normal = 400，Bold = 700</param>
        /// <param name="fontName">字体（仿宋，楷体，宋体，微软雅黑...与Excel主题字体相对应）</param>
        /// <param name="isAddBorderColor">是否增加边框颜色</param>
        /// <param name="isItalic">是否将文字变为斜体</param>
        /// <param name="isLineFeed">是否自动换行</param>
        /// <param name="isAddCellBackground">是否增加单元格背景颜色</param>
        /// <param name="fillPattern">填充图案样式(FineDots 细点，SolidForeground立体前景，isAddFillPattern=true时存在)</param>
        /// <param name="cellBackgroundColor">单元格背景颜色（当isAddCellBackground=true时存在）</param>
        /// <param name="fontColor">字体颜色</param>
        /// <param name="underlineStyle">下划线样式（无下划线[None],单下划线[Single],双下划线[Double],会计用单下划线[SingleAccounting],会计用双下划线[DoubleAccounting]）</param>
        /// <param name="typeOffset">字体上标下标(普通默认值[None],上标[Sub],下标[Super]),即字体在单元格内的上下偏移量</param>
        /// <param name="isStrikeout">是否显示删除线</param>
        /// <returns></returns>
        public HSSFCellStyle CreateStyle(HSSFWorkbook workbook, HorizontalAlignment hAlignment, VerticalAlignment vAlignment, short fontHeightInPoints, bool isAddBorder, bool boldWeight, string fontName = "宋体", bool isAddBorderColor = true, bool isItalic = false, bool isLineFeed = false, bool isAddCellBackground = false, FillPattern fillPattern = FillPattern.NoFill, short cellBackgroundColor = HSSFColor.Yellow.Index, short fontColor = HSSFColor.Black.Index, FontUnderlineType underlineStyle =
            FontUnderlineType.None, FontSuperScript typeOffset = FontSuperScript.None, bool isStrikeout = false)
        {
            HSSFCellStyle cellStyle = (HSSFCellStyle)workbook.CreateCellStyle(); //创建列头单元格实例样式
            cellStyle.Alignment = hAlignment; //水平居中
            cellStyle.VerticalAlignment = vAlignment; //垂直居中
            cellStyle.WrapText = isLineFeed;//自动换行

            //背景颜色，边框颜色，字体颜色都是使用 HSSFColor属性中的对应调色板索引，关于 HSSFColor 颜色索引对照表，详情参考：https://www.cnblogs.com/Brainpan/p/5804167.html
            //十分注意设置单元格背景色必须是FillForegroundColor和FillPattern两个属性同时设置，否则是不会显示背景颜色
            //FillForegroundColor属性实现 Excel 单元格的背景色设置
            //FillPattern 为单元格背景色的填充样式
            if (isAddCellBackground)
            {
                cellStyle.FillForegroundColor = cellBackgroundColor;//单元格背景颜色
                cellStyle.FillPattern = fillPattern;//填充图案样式(FineDots 细点，SolidForeground立体前景)
            }

            //是否增加边框
            if (isAddBorder)
            {
                //常用的边框样式
                //None(没有),Thin(细边框，瘦的),Medium(中等),Dashed(虚线),
                //Dotted(星罗棋布的),Thick(厚的),Double(双倍),Hair(头发)[上右下左顺序设置]
                cellStyle.BorderBottom = BorderStyle.Thin;
                cellStyle.BorderRight = BorderStyle.Thin;
                cellStyle.BorderTop = BorderStyle.Thin;
                cellStyle.BorderLeft = BorderStyle.Thin;
            }

            //是否设置边框颜色
            if (isAddBorderColor)
            {
                //边框颜色[上右下左顺序设置]
                cellStyle.TopBorderColor = HSSFColor.DarkGreen.Index;//DarkGreen(黑绿色)
                cellStyle.RightBorderColor = HSSFColor.DarkGreen.Index;
                cellStyle.BottomBorderColor = HSSFColor.DarkGreen.Index;
                cellStyle.LeftBorderColor = HSSFColor.DarkGreen.Index;
            }

            //设置相关字体样式
            var cellStyleFont = (HSSFFont)workbook.CreateFont(); //创建字体
            cellStyleFont.IsBold = boldWeight; //字体加粗
            cellStyleFont.FontHeightInPoints = fontHeightInPoints; //字体大小
            cellStyleFont.FontName = fontName;//字体（仿宋，楷体，宋体 ）
            cellStyleFont.Color = fontColor;//设置字体颜色
            cellStyleFont.IsItalic = isItalic;//是否将文字变为斜体
            cellStyleFont.Underline = underlineStyle;//字体下划线
            cellStyleFont.TypeOffset = typeOffset;//字体上标下标
            cellStyleFont.IsStrikeout = isStrikeout;//是否有删除线
            cellStyle.SetFont(cellStyleFont); //将字体绑定到样式

            return cellStyle;
        }
        #endregion

        #endregion
    }
}
