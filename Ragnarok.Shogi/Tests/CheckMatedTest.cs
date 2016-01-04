#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi.Tests
{
    using Sfen;

    /// <summary>
    /// 詰みや王手に関するテストを行います。
    /// </summary>
    [TestFixture()]
    internal sealed class CheckMatedTest
    {
        private static Tuple<string, string[]>[] Mate1PlyList =
        {
            CreateMate1Ply("l5kn+B/1r4g2/2g1p2p1/p2P1Lp1p/3S1p3/P1P5P/1P2P1+s2/1K1+ps4/LNNb4L b 4PNSGpgr 1", "G*4a"),
            CreateMate1Ply("l7l/4n4/p2pppn2/2+R1b3p/k1P1s1S2/+b2P1PP2/4PG2P/4K4/LP5NL b 3PG3pn2s2gr 1", "G*8e"),
            CreateMate1Ply("l8/2g3s2/n2p5/pPP2kSp1/3PP1pBN/P6Sp/2N3P2/2G5r/L4GKL1 b 3PLNSG4pbr 1", "G*5d"),
            CreateMate1Ply("3g4+R/2s6/1pnpps2p/P1kB5/5P3/2PP5/1+ln1P1P1P/3NG4/1SK5L w 4PNGR3p2lsgb 1", "G*6i"),
            CreateMate1Ply("l7l/2P6/3Spk3/3P2p1p/pp2P1sK1/2S2P2P/PP2R2P1/9/LN2G2+bL w 2P2NS3GBR3pn 1", "N*3c"),
            CreateMate1Ply("8l/1+r7/1npp2+PG1/5S2k/GKP1Pp1pp/1G1P2B1P/P1SGp3B/4+s1+s2/L+r5NL b 3PN3pln 1", "N*2f"),
            CreateMate1Ply("7nl/s+R7/1s1g1s1pp/p1pp+b1p2/1k5P1/P2N2g2/1NSP1PN1P/2K6/L7L b 6PLp2gbr 1", "L*8f"),
            CreateMate1Ply("ln1g5/1R1s1G3/pn2p2s+P/k2Knp1p1/1pGN1+bp2/2PP5/P3PP2L/9/L2+p2G2 w 2PR3pl2sb 1", "S*6c"),
            CreateMate1Ply("l2pB3+P/2gr5/1skg2+R1p/1p3B1p1/p2g5/1Pp2P2P/PlsKN4/L8/1N2P3L w 2P2NS4psg 1", "G*6h"),
            CreateMate1Ply("ln5Gl/4g1+B1k/2s4p1/p5r2/4b1pNK/6n1p/P1+p1P1PP1/L3G1S2/1+r3G1NL b 2PS7ps 1", "S*1c"),
            CreateMate1Ply("l7l/3g5/8p/1R3Gp2/p2n5/1Pp3Sk1/P5+b1P/LG2S4/KN5NL b 3PR8pn2sgb 1", "R*1f"),
            CreateMate1Ply("lns6/1k4+P2/1pp+R5/p4p1p1/5N1n1/2P6/PPLp1P2P/L1ggL3+r/K4GP2 w PN2SG2B4ps 1", "S*8h"),
            CreateMate1Ply("l1l4+Bl/5b3/1p1+N3pp/1sn1r1p2/P1kp1P3/pPp1P1P2/S2PGp1KP/1G2G1S2/+r4G1NL b 2PNSp 1", "N*8g"),
            CreateMate1Ply("ln3+R1gk/3p2Psl/p5sp1/2gP1p2p/2+b4n1/4PBn2/P4PpPP/5G2L/2P5K w 4PN2SRlg 1", "G*2h"),
            CreateMate1Ply("ln3k3/1r2g4/p1pSp1+B2/3p5/1pN6/P1P6/1P1Ps1P1+l/1SG6/1+b2K2+p1 b 3P2LG4p2nsgr 1", "G*3b"),
            CreateMate1Ply("l4S2+B/1k1G5/p2p1p2p/2p1p4/6B2/1p2l1Pp1/P2P1P2P/KS+r3G2/LN6L w 2N2SR5pn2g 1", new [] { "G*8g", "8f8g+" }),
            CreateMate1Ply("7n1/5bgk1/3p1+P1pl/6P1p/1ng2S+bP1/LP2P1N1L/3K5/1L7/1N7 w 3P3S2GR7pr 1", "R*6h"),
            CreateMate1Ply("l3+B3l/2pn2pkp/5p3/p2K1sPgP/1P4n2/4sN1r1/P2P5/2G1S1G2/L7L w 2PNSGB6pr 1", "R*6e"),
            CreateMate1Ply("ln1g4l/2s3g2/1p1p3pp/p5p2/Pk2+B2P1/1N1B1pP2/1P1S4P/2K1+p2r1/LN1G4L b P4pn2sgr 1", "8i7g"),
            CreateMate1Ply("l5Gnl/2pp3k1/p3B2p1/1p1P2s2/3b3n1/2+r3P1p/PP5S1/5G1KP/4PG2L w PN2SR5plng 1", "G*2i"),
            CreateMate1Ply("l5g2/6gs1/p6p1/4Pr2L/1p7/3P4k/PP+R1p1NP1/7S1/LN3GK2 b PLSB8p2nsgb 1", "L*1g"),
            CreateMate1Ply("l5k1l/3b5/p2p1G1S1/8p/1K2p4/P2P4P/Lpr5L/9/9 w 2P4N3S2GBR8pg 1", "G*7e"),
            CreateMate1Ply("+R1p3+P1k/2+N4gl/3p3pp/2P1+r1p2/5p1BL/P3p1KS1/1P5g1/3Pg1+b2/8L w 5PL2N3SGpn 1", new [] { "N*2d", "N*4d" }),
            CreateMate1Ply("ln3k1nB/2r6/3psK+Lp1/p1P4gL/2p3r1P/P2PP1n2/1P3P1S1/2GS1b1P1/5+n3 w 6PLSGg 1", "G*5b"),
            CreateMate1Ply("8l/l4gk2/p1n1pg3/3s1s1G1/Pp1p1ps1P/4P1p1K/LP1P1P2N/3+r2S2/1N6L w 4PN2BR2pg 1", "G*2f"),
            CreateMate1Ply("l2R1p3/6PPk/2ngp1Sn1/p1bp3+S1/2p4n1/PS1P5/1P2+b4/1KG6/LN3r3 b PL7pls2g 1", "L*1c"),
            CreateMate1Ply("lnrs+B4/9/p1ppgg1pl/1k7/P1PP1p1Pp/4S1P2/1PN1S3P/2K2P3/L2G2+r2 b PG3pl2nsb 1", "G*7d"),
            CreateMate1Ply("ln1k2B1l/9/p1ppSp2p/6s2/2P6/1P1PP4/P3+nP2P/6G2/L1S3K1L b 6PNGns2gb2r 1", "G*6b"),
            CreateMate1Ply("lnB1G1+B2/1r7/3p2pp1/2P1k3l/ppSP1p1N1/2KLS4/PP1S3+r1/2s6/LNg3P2 b 2P5pn2g 1", new [] { "3a5c", "7a5c+" }),
            CreateMate1Ply("ln5nl/4R1sk1/1pl1sG1pp/p8/3Ppp3/1KG3pP1/PP6P/1bPS5/LN6+r w SGB5png 1", "G*9e"),
            CreateMate1Ply("7nk/2B2ggsl/2Rp1p1sp/p1p6/3PKNp1P/P3PP1P1/4+r4/9/L2L5 w 4PL2NSG2psgb 1", "G*4d"),
            CreateMate1Ply("l1+Ss1p2+P/pk7/2ngp4/P1pp2gpp/1P7/1SPP5/LKG3N1P/5+r3/1NBsP3L b 3PLRpngb 1", "R*8a"),
            CreateMate1Ply("ln3+r1nl/1+R2s4/2pp3+Pp/2s3g2/p4kppP/2PPPp1s1/PS1G5/1KGB5/LN4L1b b G5pn 1", "G*5e"),
            CreateMate1Ply("lB5nl/3pg4/ps2p1spp/6p2/1G1kB4/1R2P4/P4+rPPP/7K1/LN3G1NL b 3P4pn2sg 1", "8f6f"),
            CreateMate1Ply("3+P3n1/3+R2gk1/7p1/3ps1pl1/2p2Pn1p/5G2l/1P1PP1PP1/5SSK1/+l4GN2 w 4PL2BR2pnsg 1", "G*1h"),
            CreateMate1Ply("l7l/6SSk/1+P2+b4/p5Gpp/4+Bp1n1/P3N3P/1+p1P1+r3/LG4p2/KP4RNL b 5P2pn2s2g 1", "3b2a"),
            CreateMate1Ply("l2+B4l/7r1/1p1k5/p1g2p2p/1N5n1/P2NPPP2/LPp1S1pPP/3R2GK1/4b2NL b 2PG3p3sg 1", "G*5d"),
            CreateMate1Ply("l1sb1gsn1/7p1/1pkp+S1P2/p8/3Pg1KPp/P+r2N4/4P2+s1/4G+l3/L4N3 w 8PNGBlr 1", "R*3d"),
            CreateMate1Ply("l8/2SG5/k1n2pg2/pNpp4p/1p3P3/P1PP2S1P/1PSl1G3/2KG2+p2/LN2r4 b 4PLRpns2b 1", "R*8c"),
            CreateMate1Ply("3g5/l4s+L1k/p4n1p1/1r1P1NP1P/1n1S1S1S1/1PP1P4/PK1+p1G2+p/1G3+b3/LN+b5+l b 6PGpr 1", "G*2b"),
            CreateMate1Ply("ln4Bnl/6p1k/7g1/2p5p/1p2r1PK1/2P1r4/1P1PsP1PP/2G3Sb1/+p4G1NL w PLN2SG5p 1", "2a3c"),
            CreateMate1Ply("l2k3+Nl/1s7/1p4pPp/p1N6/1P2N+b3/4L4/P3KP2P/4GNP2/L1S1P1b2 b 5PSG2ps2g2r 1", "G*6b"),
            CreateMate1Ply("ln1g1R1Gl/6g1k/pp1p3p1/6rPG/2p1+bpp2/PP3nPN1/3S4P/4P3K/L4bSNL b 5PSs 1", "S*1c"),
            CreateMate1Ply("l7l/1p1S5/4R1ggp/1K1p1pNp1/pb3k3/1S1P2P1P/PP1bPP3/7s1/L1+r2S1NL b 4PNGpng 1", "G*3e"),
            CreateMate1Ply("l6nl/2+R3g1k/4p+BsP1/5pppp/np1S5/pP1PBGP1P/3p1P1s1/PKP6/LNg+r4L w 2PNSg 1", "G*8g"),
            CreateMate1Ply("1n6l/+L5g1k/1Ppgpn1Ln/3p1bpPp/B1P4N1/8P/4PGPp1/2+p3S2/r4GK1L b 2PS3p2sr 1", "S*2a"),
            CreateMate1Ply("1n1g3n1/lGs1s2+r1/7pl/kpl2p2p/2B1G2P1/P1S5P/1P1P1P1s1/2K6/1N7 b BR8plng 1", "R*9e"),
            CreateMate1Ply("l1R1s2nl/3Pggk2/p1n2p1p1/2p3p2/3pp1B2/PPP3SPp/N4s3/1p6P/L4K2L w N2GBR3ps 1", "S*4h"),
            CreateMate1Ply("l3+B3l/9/1+P2s2pp/2S1k1S2/pn6P/1S+b1PP1P1/P8/2K6/LN6L b 5PG4p2n3g2r 1", "G*5e"),
            CreateMate1Ply("l6+P+R/4ksg2/6sp1/3r1gp2/PpPLpp1P1/L1ns2P2/1P4N2/2K6/1N2N1B2 w 4PLSGB3pg 1", "G*6h"),
            CreateMate1Ply("l1s1p3l/2k6/p5n1b/1pPpP3r/P2bKp2p/2g6/4SPPPP/5G3/LR5+pL w P2N2S2G3pn 1", "1d5d"),
            CreateMate1Ply("ln1k1n1p1/1+P3+R3/p1ppp1p2/3l1K2p/1NS4+r1/P1P3s2/1g1PPPP1P/1g6L/L1Gs2+pN+b w 2PSGB 1", "2e4e"),
            CreateMate1Ply("1+R2+P2nl/3p1kg2/2n1pp1pp/4s1L2/6pPP/1pGP1P3/nP1bP+s3/2P4R1/2K5L b 2PGBpln2sg 1", "G*4a"),
            CreateMate1Ply("7+B1/5g2S/9/p1p2kp1l/5N1p1/P4PP1n/3nPGBP+p/3+r2S2/5GKL1 b 4PG4p2ln2sr 1", "G*5d"),
            CreateMate1Ply("l6nl/6k2/3p2Bpp/2p1pGP2/pn1PP2PL/2PG1p3/PP3S3/1KG5s/LN1r5 b PG3pn2sbr 1", "G*4b"),
            CreateMate1Ply("l3s3l/4s4/p5kp1/4+Bp2p/1p5G1/2PP4P/PPN+n2P1N/9/L1K3+p1L b 6PNG2s2gb2r 1", "G*3b"),
            CreateMate1Ply("l4k1nl/6g2/2n1G2p1/p1p3p1p/3gSS3/P1P3P1P/1+rBKP1N2/3S1r3/LN6L w 5PG3psb 1", new [] { "B*5h", "S*5h" }),
            CreateMate1Ply("l1+R6/2+N6/3ss2B1/1p1pps2p/P1p1k4/SP+p+n3P1/L4+BP1K/5P+p2/R2L3NL b 5PNGp3g 1", "G*4f"),
            CreateMate1Ply("lr2b3+P/9/3S3pn/1pSp3np/P1B1NpP2/1G2p1pRP/pPkg1PS2/s8/L1K5L b 2PLNG2pg 1", "G*7f"),
            CreateMate1Ply("l1kg1b3/5s2l/1pSg1Nnn1/p1p2P1K1/3N1s2p/P1P3p2/1P1PGG2P/3p5/L1+r3P1L b 5PSBr 1", "S*8b"),
            CreateMate1Ply("ln1gr4/5pP1+S/1k1ps3p/p1snp4/1pp5P/P1PPP4/1PKG4L/B8/LNgR3N1 w 3PLSBpg 1", "G*7h"),
            CreateMate1Ply("5g2l/+R4p3/3+P3p1/1pppp1N1k/6SN1/1P1+b2sSn/P3P1NgS/5G3/+p5GK1 b 3PLB4p2lr 1", "L*1e"),
            CreateMate1Ply("l1b4nl/5p1P1/p4k1pp/2pgsNpg1/3NpP1N1/P1PG1BP1P/5S3/1+r3GK2/L2+p3RL b S4ps 1", "S*3b"),
            CreateMate1Ply("l1b4nl/5p1P1/p4k1pp/2p1sNpg1/3NgP1N1/P1P2BP1P/5S3/1+r3GK2/L2+p3RL b PS4psg 1", "S*3b"),
            CreateMate1Ply("l1b4nl/5p1P1/p4k1pp/2p1sNpg1/3NBP1N1/P1P3P1P/5S3/3+r1GK2/L2+p3RL b PSG4psg 1", "S*3b"),
            CreateMate1Ply("l6n1/7b1/2k6/p3psp2/2p+R5/PL1PPpP2/3G1P3/1KS2S1r1/L6N1 b 2PL6p2ns3gb 1", "L*7d"),
            CreateMate1Ply("1+P7/3k2rr1/1ppsnK3/ll1p1L3/4P1B2/2+lS1p2P/4G1b2/1+n2G1+n2/3P2+p2 w 3P2S2G6pn 1", new [] { "N*3a", "N*5a" }),
            CreateMate1Ply("4b3l/6+R1p/n1p3p2/4ps2P/1p5p1/2P2PkPL/+pP4Nr1/L4p2K/4GgG2 w 2P2N3SGB3pl 1", "L*1g"),
            CreateMate1Ply("6gnk/+R5+Lsl/5G3/p1p1K1p2/1pbpP1+b1p/P6lP/1P3P1S1/8L/7N1 w 4P2SGR3p2ng 1", "G*6d"),
            CreateMate1Ply("8l/1+R3g1k1/2p6/3p1g2P/p3s1b1K/5P1g1/1P1P1p3/2GS2b2/4+pL3 w 8P2L4N2SRp 1", "3h1f+"),
            CreateMate1Ply("l2b4l/9/p1n3+Rp1/1sSpN1s1p/1p3Pk2/P1pPK1pPP/1P2PG3/2+r6/L7L b 2PNSG2pn2gb 1", "G*2e"),
            CreateMate1Ply("ln1gg3+P/1ks6/pppp2ppp/9/3+B1p1P1/P1PNl4/1P1PPP2+b/L2G5/KN+r6 w PLN2SRpsg 1", "G*8h"),
            CreateMate1Ply("l4k1nl/3g5/p1n1p+Bgpp/2pp1s3/1r4sN1/2P1R3P/PP1P1P3/1BGG1+p3/LNK5L b 3PS2ps 1", "S*4b"),
            CreateMate1Ply("ln3ks2/3G1rs2/p1p+Nl2p1/3p1pb1P/1p3s2+r/P1P2P3/1P1PP1PP1/1SG2KN2/LN7 b 3PGlgb 1", "G*5a"),
            CreateMate1Ply("+R1nk3nl/1p7/p+b1SG2p1/3S1p3/6p1p/5G3/P5PPP/2+r2GSK1/L4+b1NL b PL7pnsg 1", new [] { "5c5b", "6c6b+" }),
            CreateMate1Ply("8l/5GSsk/4S2p1/1np1p1P1p/1pPn1pB2/+rN1KP4/1P1G1P3/2G6/+lN5RL b 5PS2plgb 1", "S*2a"),
            CreateMate1Ply("9/l1p5l/1+Rn6/3s2G1p/p1P1Sk3/1n2P3P/PK7/3+n5/L3R+b1+pL b 3PG7pn2s2gb 1", "G*3e"),
            CreateMate1Ply("lB4s+R1/1p1gk1+Pl1/p1np1p3/2p1p4/1s4S1n/5LKL1/P2+bPP1P1/7sg/4G4 w 4PNR3png 1", "G*3g"),
            CreateMate1Ply("l6kl/5rgBn/2nbpgsP1/p2ps2Rp/1pp2p2P/P2PS4/1PS1P1+p2/2G1G4/LNK5L b PN3p 1", "N*1c"),
            CreateMate1Ply("lr7/2g+N1gp2/p1k1s4/5pl1p/Pp1LpsN2/7PP/1P4KS1/3pg1S2/2+B3bNL w PNR6pg 1", "G*4f"),
            CreateMate1Ply("lr7/2g+N1gp2/p1k1s4/5p2p/Pp1Lpsl2/6PPP/1P4KS1/3pg1S2/2+B3bNL w NR6png 1", "G*4f"),
            CreateMate1Ply("l5+P2/9/5kB+R1/2psp1s2/n2N1p3/prPSPn3/1pNG2+p2/P1G6/LLK1+B4 b PG7plsg 1", new [] { "G*4b", "2c3b" }),
        };

        private static Tuple<string, string[]> CreateMate1Ply(string sfen, string moveSfen)
        {
            return Tuple.Create(sfen, new[] { moveSfen });
        }

        private static Tuple<string, string[]> CreateMate1Ply(string sfen, string[] moveSfens)
        {
            return Tuple.Create(sfen, moveSfens);
        }

        /// <summary>
        /// 詰み判定をまとめて行います。
        /// </summary>
        [Test]
        public void MateTestList()
        {
            foreach (var tuple in Mate1PlyList)
            {
                //Console.WriteLine(tuple.Item1);

                var board = Board.ParseSfen(tuple.Item1);
                Assert.NotNull(board);
                Assert.True(board.Validate());

                // 不詰状態の確認
                Assert.False(board.IsCheckMated());

                foreach (var moveSfen in tuple.Item2)
                {
                    var move = board.SfenToMove(moveSfen);
                    Assert.NotNull(move);
                    Assert.True(move.Validate());
                    Assert.True(board.DoMove(move));

                    var turn = board.Turn;
                    Assert.True(board.IsChecked(turn));
                    Assert.False(board.IsChecked(turn.Flip()));

                    // 詰状態の確認
                    Assert.True(board.IsCheckMated());

                    board.Undo();
                }
            }
        }

        /// <summary>
        /// 通常の詰み
        /// </summary>
        [Test()]
        public void BlackTest1()
        {
            var board = Board.ParseBod(
                "後手の持駒：飛　角　金二　歩二\n" +
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "|v香v桂v銀v金v玉v金v銀v桂v香|一\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|二\n" +
                "|v歩 ・v歩v歩v歩v歩 ・v歩v歩|三\n" +
                "| ・v歩 ・ ・ ・ ・v歩 ・ ・|四\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|五\n" +
                "| ・ ・ 歩 ・ ・ ・ ・ ・ ・|六\n" +
                "| 歩 歩 ・ 歩 ・v角 歩 歩 歩|七\n" +
                "| ・ ・ ・ ・v龍 ・ ・ ・ ・|八\n" +
                "| 香 桂 銀 ・ 玉 ・ 銀 桂 香|九\n" +
                "+---------------------------+\n" +
                "先手の持駒：なし");

            Assert.True(board.IsChecked(BWType.Black));
            Assert.False(board.IsChecked(BWType.White));
            Assert.True(board.IsCheckMated());
        }

        /// <summary>
        /// 合い駒利かずの詰み
        /// </summary>
        [Test()]
        public void BlackTest2()
        {
            var board = Board.ParseBod(
                "後手の持駒：飛　角　金二　銀　歩二\n" +
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "|v香v桂 ・v金v玉v金v銀v桂v香|一\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|二\n" +
                "|v歩 ・v歩v歩v歩v歩 ・v歩v歩|三\n" +
                "| ・v歩 ・ ・ ・ ・v歩 ・ ・|四\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|五\n" +
                "| ・ ・ 歩 ・ ・ ・ ・ ・ ・|六\n" +
                "| 歩 歩 ・ 歩v龍v馬 歩 歩 歩|七\n" +
                "| ・ ・ ・ ・ ・ ・v銀 ・ ・|八\n" +
                "| 香 桂 銀 ・ 玉 ・ ・ 桂 香|九\n" +
                "+---------------------------+\n" +
                "先手の持駒：なし");

            Assert.True(board.IsChecked(BWType.Black));
            Assert.False(board.IsChecked(BWType.White));
            Assert.True(board.IsCheckMated());
        }

        /// <summary>
        /// 合い駒が利くため詰まない
        /// </summary>
        [Test()]
        public void BlackTest3()
        {
            var board = Board.ParseBod(
                "後手の持駒：飛　金二　銀　歩二\n" +
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "|v香v桂 ・v金v玉v金v銀v桂v香|一\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|二\n" +
                "|v歩 ・v歩v歩v歩v歩 ・v歩v歩|三\n" +
                "| ・v歩 ・ ・ ・ ・v歩 ・ ・|四\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|五\n" +
                "| ・ ・ 歩 ・ ・ ・ ・ ・ ・|六\n" +
                "| 歩 歩 ・ 歩v龍v馬 歩 歩 歩|七\n" +
                "| ・ ・ ・ ・ ・ ・v銀 ・ ・|八\n" +
                "| 香 桂 銀 ・ 玉 ・ ・ 桂 香|九\n" +
                "+---------------------------+\n" +
                "先手の持駒：角");

            Assert.True(board.IsChecked(BWType.Black));
            Assert.False(board.IsChecked(BWType.White));
            Assert.False(board.IsCheckMated());
        }

        /// <summary>
        /// 後手番　通常の詰み
        /// </summary>
        [Test()]
        public void WhiteTest1()
        {
            var board = Board.ParseBod(
                "後手の持駒：角　金二　銀　歩二\n" +
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "|v香v桂 ・ ・v玉 ・v銀v桂v香|一\n" +
                "| ・ ・v金 ・ 龍 ・v金 ・ ・|二\n" +
                "|v歩 ・v歩v歩 龍v歩 ・v歩v歩|三\n" +
                "| ・v歩 ・ ・ ・ ・v歩 ・ ・|四\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|五\n" +
                "| ・ ・ 歩 ・ ・ ・ ・ ・ ・|六\n" +
                "| 歩 歩 ・ 歩 ・v馬 歩 歩 歩|七\n" +
                "| ・ ・ ・ ・ ・ ・v銀 ・ ・|八\n" +
                "| 香 桂 銀 ・ 玉 ・ ・ 桂 香|九\n" +
                "+---------------------------+\n" +
                "先手の持駒：歩\n" +
                "後手番");

            Assert.False(board.IsChecked(BWType.Black));
            Assert.True(board.IsChecked(BWType.White));
            Assert.True(board.IsCheckMated());
        }

        /// <summary>
        /// 後手番　合い駒が利くため詰まない
        /// </summary>
        [Test()]
        public void WhiteTest3()
        {
            var board = Board.ParseBod(
                "後手の持駒：銀　歩\n" +
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "|v香v桂 ・ ・v玉 ・v銀v桂v香|一\n" +
                "| ・ ・ 金 ・ ・ ・v金 ・ ・|二\n" +
                "|v歩 ・v歩v歩 龍 龍 ・v歩v歩|三\n" +
                "| ・v歩 ・ ・v歩 ・v歩 ・ ・|四\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|五\n" +
                "| ・ ・ 歩 ・ ・ ・ ・ ・ ・|六\n" +
                "| 歩 歩 ・ 歩 ・v馬 歩 歩 歩|七\n" +
                "| ・ ・ ・ ・ ・ ・v銀 ・ ・|八\n" +
                "| 香 桂 銀 ・ 玉 ・ ・ 桂 香|九\n" +
                "+---------------------------+\n" +
                "先手の持駒：角　金二　歩二\n" +
                "後手番");

            Assert.False(board.IsChecked(BWType.Black));
            Assert.True(board.IsChecked(BWType.White));
            Assert.False(board.IsCheckMated());
        }
    }
}
#endif
