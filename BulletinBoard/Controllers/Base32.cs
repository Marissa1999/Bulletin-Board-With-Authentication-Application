using System;

namespace TotpAuth {

    /// <summary>
    /// Converts strings into base32 byte arrays
    /// </summary>
    internal static class Base32 {

        /// <summary>
        /// Convert a given base32 string into an array of bytes
        /// </summary>
        internal static byte[] ToByteArray( string input ) {
            input = input.TrimEnd( '=' );
            int numBytes = input.Length * 5 / 8;
            byte[] result = new byte[ numBytes ];

            byte curByte = 0, bitsRemaining = 8;
            int mask = 0;
            int arrayIndex = 0;

            foreach ( char c in input ) {
                int ascii = CharToInt( c );

                if ( bitsRemaining > 5 ) {
                    mask = ascii << ( bitsRemaining - 5 );
                    curByte = (byte)( curByte | mask );
                    bitsRemaining -= 5;
                } else {
                    mask = ascii >> ( 5 - bitsRemaining );
                    curByte = (byte)( curByte | mask );
                    result[ arrayIndex++ ] = curByte;
                    curByte = (byte)( ascii << ( 3 + bitsRemaining ) );
                    bitsRemaining += 3;
                }
            }

            if ( arrayIndex != numBytes ) {
                result[ arrayIndex ] = curByte;
            }

            return result;
        }

        // Helper - convert a base32 character into an int
        private static int CharToInt( char c ) {
            int ascii = c;

            // upper case letters
            if ( ascii < 91 && ascii > 64 ) {
                return ascii - 65;
            }

            // lower case letters
            if ( ascii < 123 && ascii > 96 ) {
                return ascii - 97;
            }

            // digits 2 through 7
            if ( ascii < 56 && ascii > 49 ) {
                return ascii - 24;
            }
            
            throw new ArgumentException( string.Format( "Invalid base32 character {0}", c ) );
        }
    }
}
