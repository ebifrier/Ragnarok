using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Provider
{
    /// <summary>
    /// 各コミュニティレベルの情報をまとめて保持しています。
    /// </summary>
    public static class CommunityLevelTable
    {
        /// <summary>
        /// コミュニティレベルが正しいか確認します。
        /// </summary>
        private static bool ValidateLevel(int level)
        {
            return (0 < level && level < communityLevelTable.Count);
        }

        /// <summary>
        /// コミュニティレベル情報を取得します。
        /// </summary>
        public static CommunityLevelInfo GetCommunityLevelInfo(int level)
        {
            if (!ValidateLevel(level))
            {
                return null;
            }

            return communityLevelTable[level];
        }

        /// <summary>
        /// レベルからそのコミュニティに必要なプレミアム会員数を取得します。
        /// </summary>
        public static int GetNumberOfPremiums(int level)
        {
            if (!ValidateLevel(level))
            {
                return -1;
            }

            return communityLevelTable[level].NumberOfPremiums;
        }

        /// <summary>
        /// レベルからそのコミュニティの最大参加人数を取得します。
        /// </summary>
        public static int GetMaximumNumberOfMembers(int level)
        {
            if (!ValidateLevel(level))
            {
                return -1;
            }

            return communityLevelTable[level].MaximumNumberOfMembers;
        }

        /// <summary>
        /// プレミアム会員数からそのコミュニティのレベルを取得します。
        /// </summary>
        public static int GetLevelFromNumberOfPremiums(int numberOfPremiums)
        {
            var min = 1;
            var max = communityLevelTable.Count;

            while (min + 1 < max)
            {
                var i = (min + max) / 2;

                if (numberOfPremiums < communityLevelTable[i].NumberOfPremiums)
                {
                    max = i - 1;
                }
                else
                {
                    min = i;
                }
            }

            return min;
        }

        /// <summary>
        /// 枠予約に必要なポイント数を取得します。
        /// </summary>
        public static int GetPointForReserve(int level)
        {
            if (level >= 90)
            {
                return 0;
            }
            else if (level >= 69)
            {
                return 100;
            }
            else if (level >= 46)
            {
                return 300;
            }
            else
            {
                return 500;
            }
        }

        /// <summary>
        /// 割り込みに必要なポイント数を取得します。
        /// </summary>
        public static int GetPointForInterrupt(int level)
        {
            if (level >= 60)
            {
                return 0;
            }
            else if (level >= 50)
            {
                return 200;
            }
            else if (level >= 40)
            {
                return 400;
            }
            else
            {
                return 600;
            }
        }

        /// <summary>
        /// 延長に必要なポイント数を取得します。
        /// </summary>
        public static int GetPointForExtension(int level)
        {
            if (level >= 97)
            {
                return 0;
            }
            else if (level >= 86)
            {
                return 0; // 一枠だけ
            }
            else if (level >= 74)
            {
                return 100;
            }
            else if (level >= 57)
            {
                return 300;
            }
            else
            {
                return 500;
            }
        }

        /// <summary>
        /// BSPを付与できる数を取得します。
        /// </summary>
        public static int GetNumberOfBSP(int level)
        {
            if (level >= 115)
            {
                return 20;
            }
            else if (level >= 80)
            {
                return 10;
            }
            else if (level >= 54)
            {
                return 5;
            }
            else if (level >= 20)
            {
                return 20;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// シート数を取得します。
        /// </summary>
        public static int GetNumberOfSeet(int level)
        {
            if (level >= 105)
            {
                return 2000;
            }
            else if (level >= 66)
            {
                return 1500;
            }
            else
            {
                return 1000;
            }
        }

        /// <summary>
        /// アンケートが使えるかを取得します。
        /// </summary>
        public static bool CanQuestionnaire(int level)
        {
            return (level >= 25);
        }

        #region table
        /// <summary>
        /// 各レベルに必要な参加人数などを保持します。
        /// </summary>
        private static readonly List<CommunityLevelInfo> communityLevelTable =
            new List<CommunityLevelInfo>()
        {
            new CommunityLevelInfo(0, 0, 0), // ０レベルは存在しません。
            new CommunityLevelInfo(1, 1, 5),
            new CommunityLevelInfo(2, 2, 10),
            new CommunityLevelInfo(3, 3, 15),
            new CommunityLevelInfo(4, 5, 27),
            new CommunityLevelInfo(5, 7, 39),
            new CommunityLevelInfo(6, 9, 51),
            new CommunityLevelInfo(7, 12, 72),
            new CommunityLevelInfo(8, 15, 93),
            new CommunityLevelInfo(9, 19, 121),
            new CommunityLevelInfo(10, 23, 153),
            new CommunityLevelInfo(11, 28, 193),
            new CommunityLevelInfo(12, 34, 241),
            new CommunityLevelInfo(13, 41, 304),
            new CommunityLevelInfo(14, 49, 376),
            new CommunityLevelInfo(15, 58, 457),
            new CommunityLevelInfo(16, 68, 557),
            new CommunityLevelInfo(17, 80, 677),
            new CommunityLevelInfo(18, 94, 817),
            new CommunityLevelInfo(19, 110, 977),
            new CommunityLevelInfo(20, 128, 1157),
            new CommunityLevelInfo(21, 148, 1357),
            new CommunityLevelInfo(22, 170, 1577),
            new CommunityLevelInfo(23, 194, 1817),
            new CommunityLevelInfo(24, 220, 2077),
            new CommunityLevelInfo(25, 248, 2357),
            new CommunityLevelInfo(26, 278, 2657),
            new CommunityLevelInfo(27, 310, 2977),
            new CommunityLevelInfo(28, 344, 3317),
            new CommunityLevelInfo(29, 380, 3677),
            new CommunityLevelInfo(30, 418, 4057),
            new CommunityLevelInfo(31, 458, 4457),
            new CommunityLevelInfo(32, 501, 4887),
            new CommunityLevelInfo(33, 547, 5347),
            new CommunityLevelInfo(34, 596, 5837),
            new CommunityLevelInfo(35, 648, 6357),
            new CommunityLevelInfo(36, 703, 6907),
            new CommunityLevelInfo(37, 761, 7487),
            new CommunityLevelInfo(38, 822, 8097),
            new CommunityLevelInfo(39, 886, 8737),
            new CommunityLevelInfo(40, 953, 9407),
            new CommunityLevelInfo(41, 1023, 10107),
            new CommunityLevelInfo(42, 1096, 10837),
            new CommunityLevelInfo(43, 1172, 11597),
            new CommunityLevelInfo(44, 1251, 12387),
            new CommunityLevelInfo(45, 1333, 13207),
            new CommunityLevelInfo(46, 1418, 14057),
            new CommunityLevelInfo(47, 1506, 14937),
            new CommunityLevelInfo(48, 1597, 15847),
            new CommunityLevelInfo(49, 1691, 16787),
            new CommunityLevelInfo(50, 1788, 17757),

            new CommunityLevelInfo(51, 1888, 18757),
            new CommunityLevelInfo(52, 1991, 19787),
            new CommunityLevelInfo(53, 2097, 20847),
            new CommunityLevelInfo(54, 2206, 21937),
            new CommunityLevelInfo(55, 2318, 23057),
            new CommunityLevelInfo(56, 2433, 24207),
            new CommunityLevelInfo(57, 2551, 25387),
            new CommunityLevelInfo(58, 2672, 26597),
            new CommunityLevelInfo(59, 2796, 27837),
            new CommunityLevelInfo(60, 2923, 29107),
            new CommunityLevelInfo(61, 3053, 30407),
            new CommunityLevelInfo(62, 3186, 31737),
            new CommunityLevelInfo(63, 3322, 33097),
            new CommunityLevelInfo(64, 3461, 34487),
            new CommunityLevelInfo(65, 3603, 35907),
            new CommunityLevelInfo(66, 3748, 37357),
            new CommunityLevelInfo(67, 3896, 38837),
            new CommunityLevelInfo(68, 4047, 40347),
            new CommunityLevelInfo(69, 4201, 41887),
            new CommunityLevelInfo(70, 4358, 43457),
            new CommunityLevelInfo(71, 4518, 45057),
            new CommunityLevelInfo(72, 4681, 46687),
            new CommunityLevelInfo(73, 4847, 48347),
            new CommunityLevelInfo(74, 5016, 50037),
            new CommunityLevelInfo(75, 5188, 51757),
            new CommunityLevelInfo(76, 5363, 53507),
            new CommunityLevelInfo(77, 5541, 55287),
            new CommunityLevelInfo(78, 5722, 57097),
            new CommunityLevelInfo(79, 5906, 58937),
            new CommunityLevelInfo(80, 6093, 60807),
            new CommunityLevelInfo(81, 6283, 62707),
            new CommunityLevelInfo(82, 6476, 64637),
            new CommunityLevelInfo(83, 6672, 66597),
            new CommunityLevelInfo(84, 6871, 68587),
            new CommunityLevelInfo(85, 7073, 70607),
            new CommunityLevelInfo(86, 7278, 72657),
            new CommunityLevelInfo(87, 7486, 74737),
            new CommunityLevelInfo(88, 7697, 76847),
            new CommunityLevelInfo(89, 7911, 78987),
            new CommunityLevelInfo(90, 8128, 81157),
            new CommunityLevelInfo(91 ,8348, 83357),
            new CommunityLevelInfo(92, 8571, 85587),
            new CommunityLevelInfo(93, 8797, 87847),
            new CommunityLevelInfo(94, 9026, 90137),
            new CommunityLevelInfo(95, 9258, 92457),
            new CommunityLevelInfo(96, 9493, 94807),
            new CommunityLevelInfo(97, 9731, 97187),
            new CommunityLevelInfo(98, 9972, 99597),
            new CommunityLevelInfo(99, 10216, 102037),

            new CommunityLevelInfo(100, 10463, 104507),
            new CommunityLevelInfo(101, 10713, 107007),
            new CommunityLevelInfo(102, 10966, 109537),
            new CommunityLevelInfo(103, 11222, 112097),
            new CommunityLevelInfo(104, 11481, 114687),
            new CommunityLevelInfo(105, 11743, 117307),
            new CommunityLevelInfo(106, 12008, 119957),
            new CommunityLevelInfo(107, 12276, 122637),
            new CommunityLevelInfo(108, 12547, 125347),
            new CommunityLevelInfo(109, 12821, 128087),
            new CommunityLevelInfo(110, 13098, 130857),
            new CommunityLevelInfo(111, 13378, 133657),
            new CommunityLevelInfo(112, 13661, 136487),
            new CommunityLevelInfo(113, 13947, 139347),
            new CommunityLevelInfo(114, 14236, 142237),
            new CommunityLevelInfo(115, 14528, 145157),
            new CommunityLevelInfo(116, 14823, 148107),
            new CommunityLevelInfo(117, 15121, 151087),
            new CommunityLevelInfo(118, 15422, 154097),
            new CommunityLevelInfo(119, 15726, 157137),
            new CommunityLevelInfo(120, 16033, 160207),
            new CommunityLevelInfo(121, 16343, 163307),
            new CommunityLevelInfo(122, 16656, 166437),
            new CommunityLevelInfo(123, 16972, 169597),
            new CommunityLevelInfo(124, 17291, 172787),
            new CommunityLevelInfo(125, 17613, 176007),
            new CommunityLevelInfo(126, 17938, 179257),
            new CommunityLevelInfo(127, 18266, 182537),
            new CommunityLevelInfo(128, 18597, 185847),
            new CommunityLevelInfo(129, 18931, 189187),
            new CommunityLevelInfo(130, 19268, 192557),
            new CommunityLevelInfo(131, 19608, 195957),
            new CommunityLevelInfo(132, 19951, 199387),
            new CommunityLevelInfo(133, 20297, 202847),
            new CommunityLevelInfo(134, 20646, 206337),
            new CommunityLevelInfo(135, 20998, 209857),
            new CommunityLevelInfo(136, 21353, 213407),
            new CommunityLevelInfo(137, 21711, 216987),
            new CommunityLevelInfo(138, 22072, 220597),
            new CommunityLevelInfo(139, 22436, 224237),
            new CommunityLevelInfo(140, 22803, 227907),
            new CommunityLevelInfo(141, 23173, 231607),
            new CommunityLevelInfo(142, 23546, 235337),
            new CommunityLevelInfo(143, 23922, 239097),
            new CommunityLevelInfo(144, 24301, 242887),
            new CommunityLevelInfo(145, 24683, 246707),
            new CommunityLevelInfo(146, 25068, 250557),
            new CommunityLevelInfo(147, 25456, 254437),
            new CommunityLevelInfo(148, 25847, 258347),
            new CommunityLevelInfo(149, 26241, 262287),
            new CommunityLevelInfo(150, 26638, 266257),

            new CommunityLevelInfo(151, 27038, 270257),
            new CommunityLevelInfo(152, 27441, 274287),
            new CommunityLevelInfo(153, 27847, 278347),
            new CommunityLevelInfo(154, 28256, 282437),
            new CommunityLevelInfo(155, 28668, 286557),
            new CommunityLevelInfo(156, 29083, 290707),
            new CommunityLevelInfo(157, 29501, 294887),
            new CommunityLevelInfo(158, 29922, 299097),
            new CommunityLevelInfo(159, 30346, 303337),
            new CommunityLevelInfo(160, 30773, 307607),
            new CommunityLevelInfo(161, 31203, 311907),
            new CommunityLevelInfo(162, 31636, 316237),
            new CommunityLevelInfo(163, 32072, 320597),
            new CommunityLevelInfo(164, 32511, 324987),
            new CommunityLevelInfo(165, 32953, 329407),
            new CommunityLevelInfo(166, 33398, 333857),
            new CommunityLevelInfo(167, 33846, 338337),
            new CommunityLevelInfo(168, 34297, 342847),
            new CommunityLevelInfo(169, 34751, 347387),
            new CommunityLevelInfo(170, 35208, 351957),
            new CommunityLevelInfo(171, 35668, 356557),
            new CommunityLevelInfo(172, 36131, 361187),
            new CommunityLevelInfo(173, 36597, 365847),
            new CommunityLevelInfo(174, 37066, 370537),
            new CommunityLevelInfo(175, 37538, 375257),
            new CommunityLevelInfo(176, 38013, 380007),
            new CommunityLevelInfo(177, 38491, 384787),
            new CommunityLevelInfo(178, 38972, 389597),
            new CommunityLevelInfo(179, 39456, 394437),
            new CommunityLevelInfo(180, 39943, 399307),
            new CommunityLevelInfo(181, 40433, 404207),
            new CommunityLevelInfo(182, 40926, 409137),
            new CommunityLevelInfo(183, 41422, 414097),
            new CommunityLevelInfo(184, 41921, 419087),
            new CommunityLevelInfo(185, 42423, 424107),
            new CommunityLevelInfo(186, 42928, 429157),
            new CommunityLevelInfo(187, 43436, 434237),
            new CommunityLevelInfo(188, 43947, 439347),
            new CommunityLevelInfo(189, 44461, 444487),
            new CommunityLevelInfo(190, 44978, 449657),
            new CommunityLevelInfo(191, 45498, 454857),
            new CommunityLevelInfo(192, 46021, 460087),
            new CommunityLevelInfo(193, 46547, 465347),
            new CommunityLevelInfo(194, 47076, 470637),
            new CommunityLevelInfo(195, 47608, 475957),
            new CommunityLevelInfo(196, 48143, 481307),
            new CommunityLevelInfo(197, 48681, 486687),
            new CommunityLevelInfo(198, 49222, 492097),
            new CommunityLevelInfo(199, 49766, 497537),
            new CommunityLevelInfo(200, 50313, 503007),

            new CommunityLevelInfo(201, 50863, 508507),
            new CommunityLevelInfo(202, 51416, 514037),
            new CommunityLevelInfo(203, 51972, 519597),
            new CommunityLevelInfo(204, 52531, 525187),
            new CommunityLevelInfo(205, 53093, 530807),
            new CommunityLevelInfo(206, 53658, 536457),
            new CommunityLevelInfo(207, 54226, 542137),
            new CommunityLevelInfo(208, 54797, 547847),
            new CommunityLevelInfo(209, 55371, 553587),
            new CommunityLevelInfo(210, 55948, 559357),
            new CommunityLevelInfo(211, 56528, 565157),
            new CommunityLevelInfo(212, 57111, 570987),
            new CommunityLevelInfo(213, 57697, 576847),
            new CommunityLevelInfo(214, 58286, 582737),
            new CommunityLevelInfo(215, 58878, 588657),
            new CommunityLevelInfo(216, 59473, 594607),
            new CommunityLevelInfo(217, 60071, 600587),
            new CommunityLevelInfo(218, 60672, 606597),
            new CommunityLevelInfo(219, 61276, 612637),
            new CommunityLevelInfo(220, 61883, 618707),
            new CommunityLevelInfo(221, 62493, 624807),
            new CommunityLevelInfo(222, 63106, 630937),
            new CommunityLevelInfo(223, 63722, 637097),
            new CommunityLevelInfo(224, 64341, 643287),
            new CommunityLevelInfo(225, 64963, 649507),
            new CommunityLevelInfo(226, 65588, 655757),
            new CommunityLevelInfo(227, 66216, 662037),
            new CommunityLevelInfo(228, 66847, 668347),
            new CommunityLevelInfo(229, 67481, 674687),
            new CommunityLevelInfo(230, 68118, 681057),
            new CommunityLevelInfo(231, 68758, 687457),
            new CommunityLevelInfo(232, 69401, 693887),
            new CommunityLevelInfo(233, 70047, 700347),
            new CommunityLevelInfo(234, 70696, 706837),
            new CommunityLevelInfo(235, 71348, 713357),
            new CommunityLevelInfo(236, 72003, 719907),
            new CommunityLevelInfo(237, 72661, 726487),
            new CommunityLevelInfo(238, 73322, 733097),
            new CommunityLevelInfo(239, 73986, 739737),
            new CommunityLevelInfo(240, 74653, 746407),
            new CommunityLevelInfo(241, 75323, 753107),
            new CommunityLevelInfo(242, 75996, 759837),
            new CommunityLevelInfo(243, 76672, 766597),
            new CommunityLevelInfo(244, 77351, 773387),
            new CommunityLevelInfo(245, 78033, 780207),
            new CommunityLevelInfo(246, 78718, 787057),
            new CommunityLevelInfo(247, 79406, 793937),
            new CommunityLevelInfo(248, 80097, 800847),
            new CommunityLevelInfo(249, 80791, 807787),
            new CommunityLevelInfo(250, 81488, 814757),

            new CommunityLevelInfo(251, 82188, 821757),
            new CommunityLevelInfo(252, 82891, 828787),
            new CommunityLevelInfo(253, 83597, 835847),
            new CommunityLevelInfo(254, 84306, 842937),
            new CommunityLevelInfo(255, 85018, 850057),
            new CommunityLevelInfo(256, 85733, 857207),
        };
        #endregion
    }
}
