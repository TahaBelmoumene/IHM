using System;
using Metier.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

// Alias pour éviter les conflits de noms de couleurs
using QColors = QuestPDF.Helpers.Colors;

namespace IHM.Services
{
    public static class FactureExporter
    {
        public static void GenererPdf(Facture facture, string cheminFichier)
        {
            if (facture.Client == null) return;

            // CORRECTION : On utilise le chemin complet "QuestPDF.Fluent.Document"
            QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(QColors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // 1. En-tête
                    page.Header()
                        .Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text($"Facture N° {facture.Id}").SemiBold().FontSize(20).FontColor(QColors.Blue.Medium);
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
                        column.Item().Border(1).BorderColor(QColors.Grey.Medium).Padding(10).Column(c =>
                        {
                            c.Item().Text("CLIENT").SemiBold().FontSize(10).FontColor(QColors.Grey.Medium);
                            c.Item().Text($"{facture.Client.Nom} {facture.Client.Prenom}").FontSize(14).SemiBold();
                            c.Item().Text($"Tél : {facture.Client.Telephone}");
                        });

                        column.Item().Height(1, Unit.Centimetre);

                        // Tableau
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Désignation");
                                header.Cell().Element(CellStyle).AlignRight().Text("Qté");
                                header.Cell().Element(CellStyle).AlignRight().Text("Prix U.");
                                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(QColors.Black);
                                }
                            });

                            foreach (var ligne in facture.Lignes)
                            {
                                string nom = ligne.NomPiece ?? "Pièce";

                                table.Cell().Element(CellStyle).Text(nom);
                                table.Cell().Element(CellStyle).AlignRight().Text(ligne.Quantite.ToString());
                                table.Cell().Element(CellStyle).AlignRight().Text($"{ligne.PrixUnitaire} €");
                                table.Cell().Element(CellStyle).AlignRight().Text($"{(ligne.Quantite * ligne.PrixUnitaire)} €");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(QColors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });

                        column.Item().Height(1, Unit.Centimetre);

                        // Total
                        column.Item().AlignRight().Text($"TOTAL À PAYER : {facture.Total:N2} €").FontSize(18).SemiBold().FontColor(QColors.Green.Medium);
                    });

                    // 3. Pied de page
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