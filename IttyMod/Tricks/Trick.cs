using System;
using TinyLife.Objects;
using System.Collections.Generic;

namespace IttyMod.Tricks {
    /// <summary>
    ///     A class representing a type of Bit Trick.
    /// </summary>
    public abstract class Trick {

        #nullable enable
        /// <summary>
        ///     A helper class for Trick return types.
        ///     Shouldn't need to be called explicitly.
        /// </summary>
        public struct SubstitutionOrSuccess {
            public bool? success;
            public string? substitution;

            public SubstitutionOrSuccess(bool s): this() {
                success = s;
            }

            public SubstitutionOrSuccess(string s): this() {
                substitution = s;
            }

            public override int GetHashCode()
            {
                if(substitution != null)
                    return substitution.GetHashCode();
                else if(success != null) // Theoretically possible through some jank.
                    return (bool) success ? 1 : 0;
                return 0;
            }

            public static implicit operator SubstitutionOrSuccess(string s) => new SubstitutionOrSuccess(s);
            public static implicit operator SubstitutionOrSuccess(bool s) => new SubstitutionOrSuccess(s);

            public override bool Equals(object? o) => o is SubstitutionOrSuccess ? this.Equals((SubstitutionOrSuccess) o) : false;
            public bool Equals(SubstitutionOrSuccess o) => (this.success == o.success) || (this.substitution == o.substitution);

            public bool Equals(bool b) => this.success != null && this.success == b;
            public static bool operator ==(SubstitutionOrSuccess a, bool b) => a.Equals(b);
            public static bool operator !=(SubstitutionOrSuccess a, bool b) => !a.Equals(b);

            public bool Equals(string b) => this.substitution != null && this.substitution.Equals(b);
            public static bool operator ==(SubstitutionOrSuccess a, string b) => a.Equals(b);
            public static bool operator !=(SubstitutionOrSuccess a, string b) => !a.Equals(b);
        }

        #nullable enable
        /// <summary>Attempt to parse this trick.</summary>
        /// <param name="trick">
        ///     The trick magic that was routed to this trick.
        ///     This includes the name, but not the surrounding shock braces ("{!" and "!}")
        /// </param>
        /// <param name="owner">
        ///     The Person who created this Bit.
        ///     Not set if this is a "Sponsored" Bit.
        /// </param>
        /// <param name="involved">
        ///     A list of MapObjects involved in this Bit.
        /// </param>
        /// <returns>
        ///     Returns a string to replace the trick magic with the provided text.
        ///     Return true to cancel this bit but continue processing tricks.
        ///     Return false to cancel this bit and stop processing later tricks.
        /// </returns>
        public abstract SubstitutionOrSuccess parse(string trick, Person? owner, List<MapObject> involved);
    }
}