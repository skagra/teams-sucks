namespace TeamsSucks
{

   public interface IComms
   {
      int Read(byte[] buffer, int offset, int count);
      void Write(byte[] buffer, int offset, int count);
   }
}