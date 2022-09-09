namespace Mint.Substrate.Porting
{
    using System;

    public class PortableElement
    {
        private PreConvertedElement curr;
        private PreConvertedElement? last;
        private PreConvertedElement? next;

        public PortableElement(PreConvertedElement curr,
                               PreConvertedElement? last,
                               PreConvertedElement? next)
        {
            this.curr = curr;
            this.last = last;
            this.next = next;
        }

        public PreConvertedElement Current => this.curr;
        public PreConvertedElement? Last => this.last;
        public PreConvertedElement? Next => this.next;


        public override bool Equals(object? obj)
        {
            return obj is PortableElement other &&
                   this.curr.ConvertResult == other.curr.ConvertResult &&
                   this.curr.Element?.ToString() == other.curr.Element?.ToString();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.curr.ConvertResult, this.curr.Element?.ToString());
        }
    }
}
