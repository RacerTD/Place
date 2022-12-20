using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaceFiller
{
    public static class ColorPallet
    {
        /// <summary>
        /// Returns a corresponding number to a color string
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static byte ColorToNumber(string color)
        {
            return color switch
            {
                "6D001A" => 0,
                "BE0039" => 1,
                "FF4500" => 2,
                "FFA800" => 3,
                "FFD635" => 4,
                "FFF8B8" => 5,
                "00A368" => 6,
                "00CC78" => 7,
                "7EED56" => 8,
                "00756F" => 9,
                "009EAA" => 10,
                "00CCC0" => 11,
                "2450A4" => 12,
                "3690EA" => 13,
                "51E9F4" => 14,
                "493AC1" => 15,
                "6A5CFF" => 16,
                "94B3FF" => 17,
                "811E9F" => 18,
                "B44AC0" => 19,
                "E4ABFF" => 20,
                "DE107F" => 21,
                "FF3881" => 22,
                "FF99AA" => 23,
                "6D482F" => 24,
                "9C6926" => 25,
                "FFB470" => 26,
                "000000" => 27,
                "515252" => 28,
                "898D90" => 29,
                "D4D7D9" => 30,
                "FFFFFF" => 31,
                _ => 0
            };
        }

        public static byte ColorToNumber2017(string color)
        {

        }

        /// <summary>
        /// Returns a color for a byte from the 2017 data
        /// Colors might not be accurate
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string NumberToColor2017(byte number)
        {
            return number switch
            {
                0 => "FFFFFF",
                1 => "E4E4E4",
                2 => "888888",
                3 => "222222",
                4 => "FFA7D1",
                5 => "E50000",
                6 => "E59500",
                7 => "A06A42",
                8 => "E5D900",
                9 => "94E044",
                10 => "02BE01",
                11 => "00E5F0",
                12 => "0083C7",
                13 => "0000EA",
                14 => "E04AFF",
                15 => "820080",
                _ => "FFFFFF"
            };
        }

        public static string NumberToColor2022(byte number)
        {

        }
    }

    // Palette Original List in Order for 2022
    //#6D001A - Burgundy
    //#BE0039 - Dark Red
    //#FF4500 - Red --
    //#FFA800 - Orange --
    //#FFD635 - Yellow --
    //#FFF8B8 - Pale Yellow
    //#00A368 - Dark Green
    //#00CC78 - Green --
    //#7EED56 - Light Green --
    //#00756F - Dark Teal
    //#009EAA - Teal
    //#00CCC0 - Light Teal
    //#2450A4 - Dark Blue --
    //#3690EA - Blue --
    //#51E9F4 - Light Blue --
    //#493AC1 - Indigo
    //#6A5CFF - Periwinkle
    //#94B3FF - Lavender
    //#811E9F - Dark Purple --
    //#B44AC0 - Purple --
    //#E4ABFF - Pale Purple
    //#DE107F - Magenta
    //#FF3881 - Pink
    //#FF99AA - Light Pink --
    //#6D482F - Dark Brown
    //#9C6926 - Brown --
    //#FFB470 - Beige
    //#000000 - Black --
    //#515252 - Dark Grey
    //#898D90 - Gray --
    //#D4D7D9 - Light Grey --
    //#FFFFFF - White --

    // Palette for 2017

}
