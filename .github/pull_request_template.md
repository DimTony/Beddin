## What does this PR do?
Implements the property search page with location autocomplete
and filter panel (bedrooms, price range, property type).

## Related issue
Closes #42

## Changes
- `SearchBar` component with debounced input (300ms)
- `FilterPanel` with price range slider + bedroom selector
- `usePropertySearch` hook wrapping React Query
- API route: GET /api/properties with query params
- Unit tests for `usePropertySearch` hook

## Screenshots
| Before | After |
|--------|-------|
| /search was a 404 | [screenshot] |

## Testing done
- [ ] Tested on Chrome, Firefox, mobile Safari
- [ ] Unit tests pass locally (`pnpm test`)
- [ ] Linter passes (`pnpm lint`)
- [ ] No console errors in dev

## Notes for reviewer
The debounce is intentionally 300ms — feel free to raise if
the search feels sluggish. Happy to make it configurable.