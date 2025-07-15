export function getStatusColor(status) {
  const color = {
    online: "green",
    offline: "gray",
    away: "orange",
    busy: "red",
  };
  return color[status] ?? "black";
}
