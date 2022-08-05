using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.ExpeditionViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.PDFTemplates
{
    public class ExpeditionPdfTemplate
    {
        private class TableContent
        {
            public int No { get; set; }
            public string PackingList { get; set; }
            public string Password { get; set; }
            public double Weight { get; set; }
            public string Total { get; set; }
        }
        private class TableContent2
        {
            public string Total { get; set; }
            public string QtyTotal { get; set; }
            public string PriceTotal { get; set; }
        }

        public MemoryStream GeneratePdfTemplate(ExpeditionViewModel viewModel, IServiceProvider serviceProvider, int clientTimeZoneOffset/*, IGarmentDeliveryOrderFacade DOfacad*/)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
            Font normal_font1 = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 20);
            Font bold_font1 = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);

            Document document = new Document(PageSize.A4, 40, 40, 40, 40);
            document.AddHeader("Header", viewModel.code);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.PageEvent = new PDFPages();
            document.Open();


            PdfPCell cellLeftNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellRightNoBorder = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };

            Chunk chkHeader = new Chunk(" ");
            Phrase pheader = new Phrase(chkHeader);
            HeaderFooter header = new HeaderFooter(pheader, false);
            header.Border = Rectangle.NO_BORDER;
            header.Alignment = Element.ALIGN_RIGHT;
            document.Header = header;

            #region Header

            string titleString = "BON PENGELUARAN BARANG\n\n";
            Paragraph title = new Paragraph(titleString, bold_font) { Alignment = Element.ALIGN_CENTER };
            document.Add(title);
            bold_font.SetStyle(Font.NORMAL);

            //string addressString = "PT DAN LIRIS" + "\n" + "JL. Merapi No.23" + "\n" + "Banaran, Grogol, Kab. Sukoharjo" + "\n" + "Jawa Tengah 57552 - INDONESIA" + "\n" + "PO.BOX 166 Solo 57100" + "\n" + "Telp. (0271) 740888, 714400" + "\n" + "Fax. (0271) 735222, 740777";
            //Paragraph address = new Paragraph(addressString, bold_font) { Alignment = Element.ALIGN_LEFT };
            //document.Add(address);
            //bold_font.SetStyle(Font.NORMAL);




            PdfPTable tableInternNoteHeader = new PdfPTable(2);
            tableInternNoteHeader.SetWidths(new float[] { 6f, 5f });
            PdfPCell cellInternNoteHeaderLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cellInternNoteHeaderRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };

            cellInternNoteHeaderLeft.Phrase = new Phrase("PT. TABOR ANDALAN RETAILINDO", bold_font1);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderLeft);

            cellInternNoteHeaderLeft.Phrase = new Phrase("No. Bon" + "       : " + viewModel.code, normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderLeft);

            cellInternNoteHeaderRight.Phrase = new Phrase("EQUITY TOWER 15C JL JEND SUDIRMAN KAV. 52-53\nSENAYAN KEBAYORAN BARU\nJAKARTA SELATAN DKI JAKARTA\nNO NPWP : 71.157.388.1-451.000", normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderRight);

            //cellInternNoteHeaderLeft.Phrase = new Phrase("Password" + "                 : " + viewModel.password, normal_font);
            //tableInternNoteHeader.AddCell(cellInternNoteHeaderLeft);

            //cellInternNoteHeaderRight.Phrase = new Phrase("", normal_font);
            //tableInternNoteHeader.AddCell(cellInternNoteHeaderRight);

            cellInternNoteHeaderLeft.Phrase = new Phrase("Tanggal" + "       : " + viewModel.CreatedUtc.ToString("dd MMMM yyyy", new CultureInfo("id-ID")) + "\n\n\n", normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderLeft);

            //cellInternNoteHeaderRight.Phrase = new Phrase("Dari" + "                           : " + viewModel.source.name, normal_font);
            //tableInternNoteHeader.AddCell(cellInternNoteHeaderRight);

            //cellInternNoteHeaderLeft.Phrase = new Phrase("", normal_font);
            //tableInternNoteHeader.AddCell(cellInternNoteHeaderLeft);

            string destination = "";
            foreach(var i in viewModel.items)
            {
                destination = i.spkDocsViewModel.destination.name;
            }

            cellInternNoteHeaderRight.Phrase = new Phrase("Tujuan" + "                      : " + destination, normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderRight);

            cellInternNoteHeaderLeft.Phrase = new Phrase("", normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderLeft);

            cellInternNoteHeaderRight.Phrase = new Phrase("Ekspedisi" + "                  : " + viewModel.expeditionService.name, normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderRight);

            cellInternNoteHeaderLeft.Phrase = new Phrase("", normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderLeft);

            cellInternNoteHeaderRight.Phrase = new Phrase("Total Berat (Kg)" + "        : " + viewModel.weight, normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderRight);

            cellInternNoteHeaderLeft.Phrase = new Phrase("", normal_font);
            tableInternNoteHeader.AddCell(cellInternNoteHeaderLeft);

            //cellInternNoteHeaderRight.Phrase = new Phrase("Keterangan" + "      : " + "" + "\n\n", normal_font);
            //tableInternNoteHeader.AddCell(cellInternNoteHeaderRight);




            PdfPCell cellInternNoteHeader = new PdfPCell(tableInternNoteHeader); // dont remove
            tableInternNoteHeader.ExtendLastRow = false;
            tableInternNoteHeader.SpacingAfter = 10f;
            document.Add(tableInternNoteHeader);
            #endregion

            #region Table_Of_Content
            PdfPCell cellCenter = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellRight = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };
            PdfPCell cellLeft = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 };

            PdfPTable tableContent = new PdfPTable(5);
            tableContent.SetWidths(new float[] { 2f, 4f, 5f, 6.5f, 5.5f });
            cellCenter.Phrase = new Phrase("No", bold_font1);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Packing List", bold_font1);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Password", bold_font1);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Berat (Kg)", bold_font1);
            tableContent.AddCell(cellCenter);
            cellCenter.Phrase = new Phrase("Total Barang", bold_font1);
            tableContent.AddCell(cellCenter);


            //double totalPriceTotal = 0;
            double total = 0;



            List<TableContent> TableContents = new List<TableContent>();
            int index = 1;
            foreach (ExpeditionItemViewModel item in viewModel.items)
            {
                

                foreach (ExpeditionDetailViewModel detail in item.details) {
                    total += detail.sendQuantity;
                }

                TableContents.Add(new TableContent
                {
                    No = index++,
                    PackingList = item.spkDocsViewModel.packingList,
                    Password = item.spkDocsViewModel.password,
                    Weight = item.weight,
                    Total = total.ToString()
                });
                //totalPriceTotal += item.quantity * item.item.domesticSale;
                //total += item.quantity;

            }

            foreach (TableContent c in TableContents.OrderBy(o => o.No))
            {
                cellCenter.Phrase = new Phrase(c.No.ToString(), normal_font1);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase(c.PackingList, normal_font1);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase(c.Password, normal_font1);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase(c.Weight.ToString(), normal_font1);
                tableContent.AddCell(cellCenter);

                cellCenter.Phrase = new Phrase(c.Total, normal_font1);
                tableContent.AddCell(cellCenter);
            }

            PdfPCell cellContent = new PdfPCell(tableContent); // dont remove
            tableContent.ExtendLastRow = false;
            tableContent.SpacingAfter = 0f;
            document.Add(tableContent);
            #endregion
            #region Total
            PdfPTable tabletotal = new PdfPTable(2);
            tabletotal.SetWidths(new float[] { 17.5f, 5.5f });
            PdfPCell cellTotalContent = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            cellTotalContent.Phrase = new Phrase("Total", normal_font1);
            tabletotal.AddCell(cellTotalContent);
            //cellTotalContent.Phrase = new Phrase("" + total, normal_font1);
            //tabletotal.AddCell(cellTotalContent);
            cellTotalContent.Phrase = new Phrase("" + String.Format("{0:n}", total), normal_font1);
            tabletotal.AddCell(cellTotalContent);
            PdfPCell cellTotal = new PdfPCell(tabletotal); // dont remove
            tabletotal.ExtendLastRow = false;
            tabletotal.SpacingBefore = 0f;
            tabletotal.SpacingAfter = 20f;
            document.Add(tabletotal);
            #endregion
            //PdfPTable tableContent2 = new PdfPTable(3);
            //tableContent2.SetWidths(new float[] { 13f, 4.5f, 5.5f });
            //List<TableContent2> tableContents2 = new List<TableContent2>();
            //tableContents2.Add(new TableContent2
            //{
            //    Total = "Total",
            //    QtyTotal = total.ToString(),
            //    PriceTotal = String.Format("{0:n}", totalPriceTotal.ToString())
            //});
            //foreach (TableContent2 c in tableContents2)
            //{
            //    cellLeft.Phrase = new Phrase(c.Total, normal_font1);
            //    tableContent.AddCell(cellLeft);

            //    cellLeft.Phrase = new Phrase(c.QtyTotal, normal_font1);
            //    tableContent.AddCell(cellLeft);

            //    cellLeft.Phrase = new Phrase(c.PriceTotal, normal_font1);
            //    tableContent.AddCell(cellLeft);
            //}
            //PdfPCell cellContent2 = new PdfPCell(tableContent2); // dont remove
            //tableContent.ExtendLastRow = false;
            //tableContent.SpacingAfter = 20f;
            //document.Add(tableContent);

            //#region Footer

            //PdfPTable tableFooter = new PdfPTable(2);
            //tableFooter.SetWidths(new float[] { 1f, 1f });

            //PdfPTable tableFooterLeft = new PdfPTable(2);
            //tableFooterLeft.SetWidths(new float[] { 3f, 5f });

            //PdfPCell cellInternNoteFooterLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            //PdfPCell cellInternNoteFooterRight = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            ////foreach (var unit in units)
            ////{
            ////    if (unit.Value == 0)
            ////    {

            ////        cellLeftNoBorder.Phrase = new Phrase($"Total {unit.Key}", normal_font);
            ////        tableFooterLeft.AddCell(cellLeftNoBorder);
            ////        cellLeftNoBorder.Phrase = new Phrase($":   -", normal_font);
            ////        tableFooterLeft.AddCell(cellLeftNoBorder);
            ////    }
            ////    else
            ////    {
            ////        cellLeftNoBorder.Phrase = new Phrase($"Total {unit.Key}", normal_font);
            ////        tableFooterLeft.AddCell(cellLeftNoBorder);
            ////        cellLeftNoBorder.Phrase = new Phrase($":   {unit.Value.ToString("n", new CultureInfo("id-ID"))}", normal_font);
            ////        tableFooterLeft.AddCell(cellLeftNoBorder);
            ////    }
            ////}

            //PdfPTable tableFooterRight = new PdfPTable(2);
            //tableFooterRight.SetWidths(new float[] { 5f, 5f });

            ////cellLeftNoBorder.Phrase = new Phrase($"Total Harga Pokok (DPP)", normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////cellLeftNoBorder.Phrase = new Phrase($": " + totalPriceTotal.ToString("N", new CultureInfo("id-ID")), normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////cellLeftNoBorder.Phrase = new Phrase("Mata Uang", normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////cellLeftNoBorder.Phrase = new Phrase($": " + viewModel.currency.Code, normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////cellLeftNoBorder.Phrase = new Phrase("Total Harga Pokok (Rp)", normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////cellLeftNoBorder.Phrase = new Phrase($": " + total.ToString("N", new CultureInfo("id-ID")), normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////cellLeftNoBorder.Phrase = new Phrase("Total Nota Koreksi", normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////if (correctionNote != null)
            ////{
            ////    cellLeftNoBorder.Phrase = new Phrase($": " + totalcorrection.ToString("N", new CultureInfo("id-ID")), normal_font);
            ////    tableFooterRight.AddCell(cellLeftNoBorder);
            ////}
            ////else
            ////{
            ////    cellLeftNoBorder.Phrase = new Phrase($": " + 0, normal_font);
            ////    tableFooterRight.AddCell(cellLeftNoBorder);
            ////}

            ////cellLeftNoBorder.Phrase = new Phrase("Total Nota PPn", normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////cellLeftNoBorder.Phrase = new Phrase($": " + ppn.ToString("N", new CultureInfo("id-ID")), normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////cellLeftNoBorder.Phrase = new Phrase("Total Nota PPh", normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////cellLeftNoBorder.Phrase = new Phrase($": " + pph.ToString("N", new CultureInfo("id-ID")), normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////cellLeftNoBorder.Phrase = new Phrase("Total yang Harus Dibayar", normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            ////cellLeftNoBorder.Phrase = new Phrase($": " + maxtotal.ToString("N", new CultureInfo("id-ID")), normal_font);
            ////tableFooterRight.AddCell(cellLeftNoBorder);

            //PdfPCell cellFooterLeft = new PdfPCell(tableFooterLeft) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            //tableFooter.AddCell(cellFooterLeft);
            //PdfPCell cellFooterRight = new PdfPCell(tableFooterRight) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            //tableFooter.AddCell(cellFooterRight);


            //PdfPCell cellFooter = new PdfPCell(tableFooter); // dont remove
            //tableFooter.ExtendLastRow = false;
            //tableFooter.SpacingAfter = 20f;
            //document.Add(tableFooter);

            //#endregion

            #region TableSignature

            PdfPTable tableSignature = new PdfPTable(3);

            PdfPCell cellSignatureContent = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };

            cellSignatureContent.Phrase = new Phrase("Penerima/Pembeli\n\n\n\n\n\n\n(________________________________)", bold_font1);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Bagian Pengiriman\n\n\n\n\n\n\n(________________________________)", bold_font1);
            tableSignature.AddCell(cellSignatureContent);
            cellSignatureContent.Phrase = new Phrase("Yang Membuat\n\n\n\n\n\n\n(________________________________)", bold_font1);
            tableSignature.AddCell(cellSignatureContent);


            PdfPCell cellSignature = new PdfPCell(tableSignature); // dont remove
            tableSignature.ExtendLastRow = false;
            tableSignature.SpacingBefore = 20f;
            tableSignature.SpacingAfter = 20f;
            document.Add(tableSignature);

            #endregion

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }
    }
}
