use crate::point::Point;

#[derive(Debug)]
pub struct Map<'a> {
    map: Vec<&'a str>,
    max_height: usize,
    max_width: usize,
}

impl<'a> Map<'a> {
    pub(crate) fn new(map: Vec<&'a str>) -> Map<'a> {
        let max_height = map.len();
        let max_width = map.first().expect("No map found!").len();

        Map {
            map,
            max_height,
            max_width,
        }
    }
    fn coor(&self, point: &Point) -> Option<char> {
        if point.x >= self.max_width {
            return None;
        }

        if point.y >= self.max_height {
            return None;
        }

        Some(
            self.map
                .iter()
                .nth(point.y)
                .expect("failed while reading map line")
                .chars()
                .nth(point.x)
                .expect("failed while reading map col"),
        )
    }

    pub(crate) fn get_start_point(&self) -> usize {
        self.map
            .first()
            .expect("First line not found!")
            .chars()
            .enumerate()
            .find(|(_i, c)| *c == 'S')
            .map(|(i, _c)| i)
            .expect("Couldnt find starting point!")
    }

    pub(crate) fn check_path(
        &self,
        start_point: Point,
        check_start_points: &mut Vec<Point>,
    ) -> u32 {
        let mut i = 0;
        while start_point.y + i < self.max_height {
            let evaluated_point = Point {
                x: start_point.x,
                y: start_point.y + i,
            };

            if check_start_points.contains(&evaluated_point) {
                return 0;
            }
            check_start_points.push(evaluated_point.clone());

            let val = match self.coor(&evaluated_point) {
                Some(val) => val,
                None => break,
            };

            if val != '^' {
                i += 1;
                continue;
            }

            let left_val = if start_point.x > 0 {
                let left_point = Point {
                    x: evaluated_point.x - 1,
                    ..evaluated_point
                };
                self.coor(&left_point)
                    .filter(|c| *c != '^')
                    .map(|_| {
                        self.check_path(
                            Point {
                                x: evaluated_point.x - 1,
                                y: evaluated_point.y,
                            },
                            check_start_points,
                        )
                    })
                    .unwrap_or_default()
            } else {
                0
            };

            let right_point = Point {
                x: evaluated_point.x + 1,
                ..evaluated_point
            };
            let right_val = self
                .coor(&right_point)
                .filter(|c| *c != '^')
                .map(|_| {
                    self.check_path(
                        Point {
                            x: evaluated_point.x + 1,
                            y: evaluated_point.y,
                        },
                        check_start_points,
                    )
                })
                .unwrap_or_default();

            return 1 + left_val + right_val;
        }

        0
    }
}
