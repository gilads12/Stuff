using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Stuff.Cobs
{
    public static class COBS
    {
        /// <summary>
        /// The encoded data will contain only values from 0x01 to 0xFF.
        /// </summary>
        /// <param name="Input">An array up to 254 bytes in length</param>
        /// <returns>Returns the encoded input</returns>
        public static IEnumerable<byte> Encode(IEnumerable<byte> Input)
        {
            if (Input == null)
                return null;

            var result = new List<byte>();
            int distanceIndex = 0;
            byte distance = 1;  // Distance to next zero
            byte currentDistance = distance;

            foreach (var i in Input)
            {
                // If we encounter a zero (the frame delimiter)
                if (i == 0)
                {
                    // Write the value of the distance to the next zero back in output where we last saw a zero
                    result.Insert(distanceIndex, distance);

                    // Set the distance index to the latest index plus one
                    distanceIndex = result.Count;

                    // Reset the value which indicates the distance to the next zero (the frame delimiter)
                    distance = 1;
                }
                else
                {
                    // Otherwise simply add the next value to the result
                    result.Add(i);

                    // Increment the distance to the next zero
                    distance++;
                    currentDistance = distance;

                    // Check for maximum distance value
                    if (distance == 0xFF)
                    {
                        // Set the distance variable to its maximum value
                        result.Insert(distanceIndex, distance);

                        // Set the distance index to the latest index plus one
                        distanceIndex = result.Count;

                        // Reset the value which indicates the distance to the next zero (the frame delimiter)
                        distance = 1;
                    }
                }
            }

            // If the packet hasn't reached the maximum size
            if (currentDistance % 255 != 0 && result.Count > 0)
                // Add the last distance variable
                result.Insert(distanceIndex, distance);

            // Return with the result
            return result;
        }

        /// <summary>
        /// The decoded data will be restored with all zeros which were removed
        /// during the decoding process.
        /// </summary>
        /// <param name="Input">A COBS encoded array</param>
        /// <returns>Returns the decoded input</returns>
        public static IEnumerable<byte> Decode(IEnumerable<byte> Input)
        {
            if (Input == null)
                return null;

            var input = Input.ToArray();
            var result = new List<byte>();
            int distanceIndex = 0;
            byte distance = 1;  // Distance to next zero

            // Continue decoding which the next index is valid
            while (distanceIndex < input.Length)
            {
                // Get the next distance value
                distance = input[distanceIndex];

                // Ensure the input is formatted correctly (distanceIndex + distance)
                if (input.Length < distanceIndex + distance || distance < 1)
                {
                    Trace.WriteLine("Consistent Overhead Byte Stuffing failed to parse an input.");
                    return new List<byte>();
                }

                // Add the range of byte up to the next zero
                if (distance > 1)
                {
                    for (byte i = 1; i < distance; i++)
                        result.Add(input[distanceIndex + i]);
                }

                // Determine the next distance index (doing this here assists the below if)
                distanceIndex += distance;

                // Add the original zero back
                if (distance < 0xFF && distanceIndex < input.Length)
                    result.Add(0);
            }

            return result;
        }
    }
}
