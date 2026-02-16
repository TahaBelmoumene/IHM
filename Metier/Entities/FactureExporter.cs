using Metier.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Reflection.Metadata;

namespace IHM.Services
{
    public static class FactureExporter
    {
        public static void GenererPdf(Facture facture, string cheminFichier)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // 1. En-tête (Header)
                    page.Header()
                        .Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text($"Facture N° {facture.Id}").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                                column.Item().Text($"Date : {facture.DateEmission:dd/MM/yyyy}");
                            });

                            row.ConstantItem(150).Column(column =>
                            {
                                column.Item().Text("Garage Farsi").SemiBold();
                                column.Item().Text("123 Rue de la Mécanique");
                                column.Item().Text("75000 Paris");
                            });
                        });

                    // 2. Contenu
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                    {
                        // Info Client
                        // Remplacement de Colors.Grey.Light par Colors.Grey.Lighten2 ou Medium
                        column.Item().Border(1).BorderColor(Colors.Grey.Medium).Padding(10).Column(c =>
                        {
                            c.Item().Text("CLIENT").SemiBold().FontSize(10).FontColor(Colors.Grey.Medium);
                            if (facture.Client != null) // Sécurité null
                            {
                                c.Item().Text($"{facture.Client.Nom} {facture.Client.Prenom}").FontSize(14).SemiBold();
                                c.Item().Text($"Tél : {facture.Client.Telephone}");
                            }
                        });

                        column.Item().Height(1, Unit.Centimetre);

                        // Tableau
                        column.Item().Table(table =>
                        {
                            // CORRECTION : RelativeColumn et ConstantColumn
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();     // Nom
                                columns.ConstantColumn(50);   // Qté
                                columns.ConstantColumn(80);   // Prix
                                columns.ConstantColumn(80);   // Total
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Désignation");
                                header.Cell().Element(CellStyle).AlignRight().Text("Qté");
                                header.Cell().Element(CellStyle).AlignRight().Text("Prix U.");
                                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            foreach (var ligne in facture.Lignes)
                            {
                                table.Cell().Element(CellStyle).Text(ligne.NomPiece);
                                table.Cell().Element(CellStyle).AlignRight().Text(ligne.Quantite.ToString());
                                table.Cell().Element(CellStyle).AlignRight().Text($"{ligne.PrixUnitaire} €");
                                table.Cell().Element(CellStyle).AlignRight().Text($"{(ligne.Quantite * ligne.PrixUnitaire)} €");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });

                        column.Item().Height(1, Unit.Centimetre);

                        column.Item().AlignRight().Text($"TOTAL À PAYER : {facture.Total:N2} €").FontSize(18).SemiBold().FontColor(Colors.Green.Medium);
                    });

                    // 3. Footer
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(cheminFichier);
        }
    }
}