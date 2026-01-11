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
        paths_from_here: &mut std::collections::HashMap<Point, u64>,
        checked_start_points: &mut Vec<Point>,
    ) -> u32 {
        let mut i = 0;
        while start_point.y + i < self.max_height {
            let evaluated_point = Point {
                x: start_point.x,
                y: start_point.y + i,
            };

            if paths_from_here.contains_key(&evaluated_point) {
                paths_from_here.insert(
                    start_point,
                    paths_from_here.get(&evaluated_point).copied().unwrap(),
                );
                return 0;
            }
            // UNCOMMENT THIS TO FIX PART 1 BUT BREAK PART 2
            // paths_from_here.insert(evaluated_point.clone(), 0);

            let val = match self.coor(&evaluated_point) {
                Some(val) => val,
                None => return 0,
            };

            if val != '^' {
                i += 1;
                continue;
            }

            let mut descendants_value: u32 = 0;
            let left_point = Point {
                x: evaluated_point.x - 1,
                ..evaluated_point
            };
            if start_point.x > 0 {
                if self.coor(&left_point) != Some('^') {
                    descendants_value +=
                        self.check_path(left_point.clone(), paths_from_here, checked_start_points);
                }
            }

            let right_point = Point {
                x: evaluated_point.x + 1,
                ..evaluated_point
            };
            if self.coor(&right_point) != Some('^') {
                descendants_value +=
                    self.check_path(right_point.clone(), paths_from_here, checked_start_points);
            }

            let score = 1 + descendants_value;
            let child_value = paths_from_here.get(&left_point).unwrap()
                + paths_from_here.get(&right_point).unwrap();
            paths_from_here.insert(start_point.clone(), child_value);
            return score;
        }

        paths_from_here.insert(start_point, 1);
        0
    }
}
