using Microsoft.AspNetCore.Mvc;
using BuberBreakfast.Contracts.Breakfast;
using ErrorOr;


public class BreakfastController : ApiController
{
  private readonly IBreakfastService _breakfastService;

  public BreakfastController(IBreakfastService breakfastService)
  {
    _breakfastService = breakfastService;
  }

  [HttpPost]
  public IActionResult CreateBreakfast(CreateBreakfastRequest request)
    {
        var breakfast = new Breakfast(
          Guid.NewGuid(),
          request.Name,
          request.Description,
          request.StartDateTime,
          request.EndDateTime,
          DateTime.UtcNow,
          request.Savory,
          request.Sweet
        );

        ErrorOr<Created> createBreakfastResult = _breakfastService.CreateBreakfast(breakfast);

        if (createBreakfastResult.IsError)
        {
            return Problem(createBreakfastResult.Errors);
        };
        return CreatedAsGetBreakfast(breakfast);
    }



    [HttpGet("{id:guid}")]
  public IActionResult GetBreakfast(Guid id)
    {
        ErrorOr<Breakfast> getBreakfastResult = _breakfastService.GetBreakfast(id);

        return getBreakfastResult.Match(
          breakfast => Ok(MapBreakfastResponse(breakfast)),
          errors => Problem(errors)
        );
    }

    

    [HttpPut("{id:guid}")]
  public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
  {
    var breakfast = new Breakfast(
      id,
      request.Name,
      request.Description,
      request.StartDateTime,
      request.EndDateTime,
      DateTime.UtcNow,
      request.Savory,
      request.Sweet
    );

    ErrorOr<UpsertedBreakfast> upsertBreakfastResult = _breakfastService.UpsertBreakfast(breakfast);

    //TODO return 201 ij a new breakfast was created
    return upsertBreakfastResult.Match(
      upserted => upserted.IsNewlyCreated ? CreatedAsGetBreakfast(breakfast) : NoContent(),
      errors => Problem(errors)
    );
  }

  [HttpDelete("{id:guid}")]
  public IActionResult DeleteBreakfast(Guid id)
  {
    ErrorOr<Deleted> deleteBreakfastResult = _breakfastService.DeleteBreakfast(id);

    return deleteBreakfastResult.Match(
      deleted => NoContent(),
      errors => Problem(errors)
    );
  }

  private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
    {
        return new BreakfastResponse(
          breakfast.Id,
          breakfast.Name,
          breakfast.Description,
          breakfast.StartDateTime,
          breakfast.EndDateTime,
          breakfast.LastModifiedDateTime,
          breakfast.Savory,
          breakfast.Sweet
        );
    }

  private IActionResult CreatedAsGetBreakfast(Breakfast breakfast)
    {
      return CreatedAtAction(
        actionName: nameof(GetBreakfast),
        routeValues: new { id = breakfast.Id },
        value: MapBreakfastResponse(breakfast));
    }
}
