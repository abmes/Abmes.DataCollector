using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Abmes.Utils.AspNetCore;

public static class ControllerBaseExtensions
{
    public static ActionResult InvalidStateResult(this ControllerBase controller)
    {
        var errorMessages =
            string.Join(
                Environment.NewLine,
                controller.ModelState.SelectMany(x => x.Value?.Errors.Select(e => e.ErrorMessage) ?? [])
            );

        return controller.StatusCode(StatusCodes.Status400BadRequest, errorMessages);
    }

    public static ActionResult MultiStatusActionResult<T>(this ControllerBase controller, IEnumerable<T> result)
    {
        result = result.ToList();

        return
            result.Any()
            ? controller.StatusCode(StatusCodes.Status207MultiStatus, result)
            : controller.StatusCode(StatusCodes.Status204NoContent, null);
    }
}
