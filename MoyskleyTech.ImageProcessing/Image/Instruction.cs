namespace MoyskleyTech.ImageProcessing.Image
{
    internal class Instruction
    {
        private string instruction;
        private object[] data;

        public Instruction(string v , params object[] pdata)
        {
            this.instruction = v;
            this.data = pdata;
        }
        public string ToSVG()
        {
            Pixel p;
            switch ( instruction )
            {
                case "line":
                    p = (Pixel)data[4];
                    return "<line x1 = \"" + data[0] + "\" y1 = \"" + data[1] + "\" x2 = \"" + data[2] + "\" y2 = \"" + data[3] + "\" style = \"stroke:rgba("+p.R+ "," + p.G + "," + p.B + "," + ((double)p.A/255) + ");stroke-width:1\" />";
                case "circle":
                    p = ( Pixel ) data[4];
                    return "<circle cx=\"" + data[0] + "\" cy=\"" + data[1] + "\" r=\"" + data[2] + "\" stroke=\"rgba(" + p.R + " , " + p.G + " , " + p.B + " , " + ( ( double ) p.A / 255 ) + ")\" stroke-width=" + data[3] + " />";
                case "circleF":
                    p = ( Pixel ) data[3];
                    return "<circle cx=\"" + data[0] + "\" cy=\"" + data[1] + "\" r=\"" + data[2] + "\" fill=\"rgba(" + p.R + " , " + p.G + " , " + p.B + " , " + ( ( double ) p.A / 255 ) + ")\" />";
                case "text":
                    //<text x="0" y="15" fill="red">I love SVG!</text>
                    p = ( Pixel ) data[3];
                    return "<text x=\"" + data[1] + "\" y= \"" + data[1] + "\" fill=\"rgba(" + p.R + " , " + p.G + " , " + p.B + " , " + ( ( double ) p.A / 255 ) + ")\">"+data[0]+"</text>";
            }
            return "";
        }
    }
}