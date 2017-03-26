using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Bilten.Domain;

namespace Bilten.Test
{
    public class SudijeParser
    {
        private List<object[]> sudijskeUloge;
        public List<object[]> SudijskeUloge
        {
            get { return sudijskeUloge; }
        }
        
        public SudijeParser()
        {

        }

        public void parse(string fileName)
        {
            sudijskeUloge = new List<object[]>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                string[] spraveNazivi = new string[] { "Floor", "Pommel Horse", "Rings",
                    "Vault", "Parallel Bars", "High Bar", "Uneven Bars", "Beam" };
                Sprava[] sprave = new Sprava[] { Sprava.Parter, Sprava.Konj,
                    Sprava.Karike, Sprava.Preskok, Sprava.Razboj, Sprava.Vratilo,
                    Sprava.DvovisinskiRazboj, Sprava.Greda };

                bool superiorJury = false;
                Sprava sprava = Sprava.Undefined;

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim() == String.Empty)
                        continue;

                    if (line.StartsWith("Superior Jury"))
                    {
                        superiorJury = true;
                        sprava = Sprava.Undefined;
                        continue;
                    }

                    bool found = false;
                    for (int i = 0; i < spraveNazivi.Length; i++)
                    {
                        if (line.StartsWith(spraveNazivi[i]))
                        {
                            found = true;
                            sprava = sprave[i];
                            superiorJury = false;
                            break;
                        }
                    }
                    if (found)
                        continue;

                    string ime;
                    string prezime;
                    string kod;
                    string uloga;

                    parseSudijskaUloga(line, superiorJury, out ime, out prezime,
                        out kod, out uloga);
                    sudijskeUloge.Add(new object[] { ime, prezime, kod, sprava,
                        parseSudijskaUloga(uloga) });
                }
            }
        }

        private void parseSudijskaUloga(string line, bool superiorJury, out string ime,
            out string prezime, out string kod, out string uloga)
        {
            line = line.Trim();

            uloga = String.Empty;
            int index;
            if (!superiorJury)
            {
                index = line.IndexOf('\t');
                uloga = line.Substring(0, index);
                line = line.Remove(0, index).Trim();
            }

            index = line.IndexOf('\t');
            prezime = line.Substring(0, index);
            line = line.Remove(0, index).Trim();

            index = line.IndexOf('\t');
            ime = line.Substring(0, index);
            line = line.Remove(0, index).Trim();

            if (superiorJury || uloga == "D1")
            { 
                // remove UEG
                line = line.Remove(0, 3).Trim();
            }

            kod = line.Substring(0, 3);
            line = line.Remove(0, 3).Trim();

            if (superiorJury)
                uloga = line.Trim();
        }

        private SudijskaUloga parseSudijskaUloga(string uloga)
        {
            switch (uloga)
            {
                case "President":
                    return SudijskaUloga.PredsednikGlavnogSudijskogOdbora;
                case "":
                    return SudijskaUloga.ClanGlavnogSudijskogOdbora;
                case "Floor Supervisor":
                    return SudijskaUloga.ParterKontrolor;
                case "Pommel Horse Supervisor":
                    return SudijskaUloga.KonjKontrolor;
                case "Rings Supervisor":
                    return SudijskaUloga.KarikeKontrolor;
                case "Vault Supervisor":
                    return SudijskaUloga.PreskokKontrolor;
                case "Parallel Bars Supervisor":
                    return SudijskaUloga.RazbojKontrolor;
                case "High Bar Supervisor":
                    return SudijskaUloga.VratiloKontrolor;
                case "Uneven Bars Supervisor":
                    return SudijskaUloga.DvovisinskiRazbojKontrolor;
                case "Beam Supervisor":
                    return SudijskaUloga.GredaKontrolor;
                case "D1":
                    return SudijskaUloga.D1;
                case "D2":
                    return SudijskaUloga.D2;
                case "E1":
                    return SudijskaUloga.E1;
                case "E2":
                    return SudijskaUloga.E2;
                case "E3":
                    return SudijskaUloga.E3;
                case "E4":
                    return SudijskaUloga.E4;
                case "E5":
                    return SudijskaUloga.E5;
                case "E6":
                    return SudijskaUloga.E6;
                case "Timer 1":
                    return SudijskaUloga.MeracVremena1;
                case "Timer 2":
                    return SudijskaUloga.MeracVremena2;
                case "Line 1":
                    return SudijskaUloga.LinijskiSudija1;
                case "Line 2":
                    return SudijskaUloga.LinijskiSudija2;
                case "Line 3":
                    return SudijskaUloga.LinijskiSudija3;
                case "Line 4":
                    return SudijskaUloga.LinijskiSudija4;
                default:
                    throw new ArgumentException("Invalid sudijska uloga.");
            }
        }

    }
}
