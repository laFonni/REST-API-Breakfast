using ErrorOr;

public static class Errors
{
  public static class Breakfast
  {
    public static Error NotFound => Error.NotFound(
      code: "Breakfast.NotFound",
      description: "The breakfast was not found."
    );
  }
}