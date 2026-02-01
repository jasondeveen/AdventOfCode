use std::{collections::HashMap, fs, time::SystemTime};

#[derive(Debug, PartialEq, Hash, Eq, Clone, Copy)]
struct Point {
    x: u32,
    y: u32,
}

#[derive(Debug)]
struct Rectangle<'a> {
    opposing_corners: Vec<&'a Point>,
    size: u32,
    l: u32,
    t: u32,
    r: u32,
    b: u32,
}

impl<'a> Rectangle<'a> {
    fn new_from_tuple(opposing_corners_tuple: (&'a Point, &'a Point)) -> Rectangle<'a> {
        let opposing_corners = vec![opposing_corners_tuple.0, opposing_corners_tuple.1];
        Rectangle::new(opposing_corners)
    }
    fn new(opposing_corners: Vec<&'a Point>) -> Rectangle<'a> {
        if opposing_corners.len() != 2 {
            panic!(
                "Invalid rectangle! number of points: {}",
                opposing_corners.len()
            );
        }
        Rectangle {
            size: (opposing_corners[0].x.abs_diff(opposing_corners[1].x) + 1)
                * (opposing_corners[0].y.abs_diff(opposing_corners[1].y) + 1),
            l: opposing_corners.iter().map(|c| c.x).min().unwrap(),
            t: opposing_corners.iter().map(|c| c.y).min().unwrap(),
            r: opposing_corners.iter().map(|c| c.x).max().unwrap(),
            b: opposing_corners.iter().map(|c| c.y).max().unwrap(),
            opposing_corners,
        }
    }
}

#[derive(Debug)]
struct Area {
    prefix: HashMap<Point, u32>,
}

impl Area {
    fn contains(&self, rect: Rectangle) -> bool {
        let mut prefix_score = 0;

        match self.prefix.get(&Point {
            x: rect.r,
            y: rect.b,
        }) {
            Some(prefix_self) => prefix_score += prefix_self,
            None => (),
        }

        if rect.l > 0 {
            match self.prefix.get(&Point {
                x: rect.l - 1,
                y: rect.b,
            }) {
                Some(prefix_left) => prefix_score -= prefix_left,
                None => (),
            }
        }

        if rect.t > 0 {
            match self.prefix.get(&Point {
                x: rect.r,
                y: rect.t - 1,
            }) {
                Some(prefix_above) => prefix_score -= prefix_above,
                None => (),
            }
        }

        if rect.t > 0 && rect.l > 0 {
            match self.prefix.get(&Point {
                x: rect.l - 1,
                y: rect.b - 1,
            }) {
                Some(prefix_above_left) => prefix_score += prefix_above_left,
                None => (),
            }
        }

        prefix_score == rect.size
    }

    fn new(points: &[Point]) -> Area {
        Area {
            prefix: compute_prefix(points),
        }
    }
}

fn main() {
    let now = SystemTime::now();
    let args: Vec<String> = std::env::args().collect();
    let points: Vec<Point> = fs::read_to_string(&args[1])
        .expect("Couldnt find inputfile")
        .lines()
        .map(|str| str.to_owned())
        .map(|s| {
            let parts: Vec<&str> = s.split(',').collect();
            Point {
                x: parts[0].trim().parse::<u32>().unwrap(),
                y: parts[1].trim().parse::<u32>().unwrap(),
            }
        })
        .collect();

    let rectangles = build_distances_vec(&points);
    let biggest = rectangles.first().unwrap();

    println!("elapsed: {:?}", now.elapsed().unwrap());
    println!(
        "PART1: Biggest rectangle: size {} between points {:?} and {:?}",
        biggest.1, biggest.0.0, biggest.0.1
    );

    let area = Area::new(&points);
    let biggest_green = rectangles
        .iter()
        .find(|r| area.contains(Rectangle::new_from_tuple(r.0)))
        .unwrap();

    println!("elapsed: {:?}", now.elapsed().unwrap());
    println!(
        "PART2: Biggest rectangle in green/red: size {} between points {:?} and {:?}",
        biggest_green.1, biggest_green.0.0, biggest_green.0.1
    );
}

fn get_area(p1: &Point, p2: &Point) -> u64 {
    (p1.x.abs_diff(p2.x) + 1) as u64 * (p1.y.abs_diff(p2.y) + 1) as u64
}

fn build_distances_vec(points: &Vec<Point>) -> Vec<((&Point, &Point), u64)> {
    let mut rectangles = HashMap::new();
    for p1 in points {
        for p2 in points {
            if *p1 == *p2 {
                continue;
            }
            if rectangles.contains_key(&(p2, p1)) {
                continue;
            }
            rectangles.insert((p1, p2), get_area(p1, p2));
        }
    }

    let mut vec: Vec<((&Point, &Point), u64)> =
        rectangles.into_iter().map(|(k, v)| (k, v)).collect();
    vec.sort_by(|r1, r2| r2.1.cmp(&r1.1));

    vec
}

fn compute_prefix(corners: &[Point]) -> HashMap<Point, u32> {
    let green_or_red_cells: Vec<Point> = get_painted_cells(corners);

    let max_x = green_or_red_cells.iter().map(|p| p.x).max().unwrap() as usize;
    let min_x = green_or_red_cells.iter().map(|p| p.x).min().unwrap() as usize;
    let max_y = green_or_red_cells.iter().map(|p| p.y).max().unwrap() as usize;
    let min_y = green_or_red_cells.iter().map(|p| p.y).min().unwrap() as usize;

    let mut prefix = HashMap::new();

    for j in min_y..max_y as usize + 1 {
        for i in min_x..max_x as usize + 1 {
            let mut prefix_value: u32 = 0;
            let cp = Point {
                x: i as u32,
                y: j as u32,
            };

            if green_or_red_cells.iter().any(|p| *p == cp) {
                prefix_value += 1;
            }

            if cp.y > 0 {
                match prefix.get(&Point {
                    x: cp.x,
                    y: cp.y - 1,
                }) {
                    Some(prefix_above) => prefix_value += prefix_above,
                    None => (),
                }
            }

            if cp.x > 0 {
                match prefix.get(&Point {
                    x: cp.x - 1,
                    y: cp.y,
                }) {
                    Some(prefix_left) => prefix_value += prefix_left,
                    None => (),
                }
            }

            if cp.x > 0 && cp.y > 0 {
                match prefix.get(&Point {
                    x: cp.x - 1,
                    y: cp.y - 1,
                }) {
                    Some(prefix_left_above) => prefix_value -= prefix_left_above,
                    None => (),
                }
            }

            prefix.insert(cp, prefix_value);
        }
    }

    prefix
}

fn get_painted_cells(corners: &[Point]) -> Vec<Point> {
    let edges = get_edges(corners);
    get_filled_region(edges)
}

fn get_filled_region(edges: Vec<Point>) -> Vec<Point> {
    let mut filled_region = Vec::new();
    let largest_y = edges
        .iter()
        .fold(0, |acc, p| if p.y > acc { p.y } else { acc });

    for j in 0..largest_y + 1 {
        let line_edges: Vec<Point> = edges.iter().filter(|e| e.y == j).copied().collect();
        if line_edges.len() == 0 {
            continue;
        }

        let line_positions: Vec<u32> = line_edges.iter().map(|e| e.x).collect();
        let line_outer_edges: (u32, u32) = (
            line_positions.iter().min().unwrap().clone(),
            line_positions.iter().max().unwrap().clone(),
        );

        for i in line_outer_edges.0..line_outer_edges.1 + 1 {
            filled_region.push(Point { x: i, y: j });
        }
    }

    filled_region
}

fn get_edges(corners: &[Point]) -> Vec<Point> {
    let mut painted_cells = Vec::new();

    for (i, _) in corners.iter().enumerate() {
        let current_corner: Point = corners[i];
        let next_corner;
        if i < corners.len() - 1 {
            next_corner = corners[i + 1];
        } else {
            next_corner = corners[0];
        }

        if current_corner.x == next_corner.x {
            let mut y_offset: i32 = 0;
            let step: i32 = if current_corner.y < next_corner.y {
                1
            } else {
                -1
            };
            while current_corner.y.strict_add_signed(y_offset) != next_corner.y {
                painted_cells.push(Point {
                    x: current_corner.x,
                    y: current_corner.y.strict_add_signed(y_offset),
                });
                y_offset += step;
            }
        } else if current_corner.y == next_corner.y {
            let mut x_offset: i32 = 0;
            let step: i32 = if current_corner.x < next_corner.x {
                1
            } else {
                -1
            };
            while current_corner.x.strict_add_signed(x_offset) != next_corner.x {
                painted_cells.push(Point {
                    x: current_corner.x.strict_add_signed(x_offset),
                    y: current_corner.y,
                });
                x_offset += step;
            }
        } else {
            panic!(
                "Cant connect corners! c1: {:#?} c2: {:#?}",
                current_corner, next_corner
            );
        }
    }

    painted_cells
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_get_painted_cells() {
        let test_cases = [
            (
                vec![
                    Point { x: 7, y: 1 },
                    Point { x: 11, y: 1 },
                    Point { x: 11, y: 7 },
                    Point { x: 7, y: 7 },
                ],
                20,
                35,
            ),
            (
                vec![
                    Point { x: 7, y: 1 },
                    Point { x: 11, y: 1 },
                    Point { x: 11, y: 7 },
                    Point { x: 9, y: 7 },
                    Point { x: 9, y: 5 },
                    Point { x: 2, y: 5 },
                    Point { x: 2, y: 3 },
                    Point { x: 7, y: 3 },
                ],
                4 + 6 + 2 + 2 + 7 + 2 + 5 + 2,
                5 + 5 + 10 + 10 + 10 + 3 + 3,
            ),
        ];

        for (corners, expected_circ, expected_area) in test_cases {
            let circ = get_edges(&corners);
            assert_eq!(circ.len(), expected_circ, "circumference failed");
            let region = get_filled_region(circ);
            assert_eq!(region.len(), expected_area, "area failed");
        }
    }

    #[test]
    fn test_get_prefix() {
        let test_cases = [
            (
                vec![
                    Point { x: 1, y: 0 },
                    Point { x: 3, y: 0 },
                    Point { x: 3, y: 1 },
                    Point { x: 1, y: 1 },
                ],
                HashMap::from([
                    (Point { x: 1, y: 0 }, 1),
                    (Point { x: 2, y: 0 }, 2),
                    (Point { x: 3, y: 0 }, 3),
                    (Point { x: 1, y: 1 }, 2),
                    (Point { x: 2, y: 1 }, 4),
                    (Point { x: 3, y: 1 }, 6),
                ]),
            ),
            (
                vec![
                    Point { x: 1, y: 0 },
                    Point { x: 3, y: 0 },
                    Point { x: 3, y: 1 },
                    Point { x: 2, y: 1 },
                    Point { x: 2, y: 2 },
                    Point { x: 1, y: 2 },
                ],
                HashMap::from([
                    (Point { x: 1, y: 0 }, 1),
                    (Point { x: 2, y: 0 }, 2),
                    (Point { x: 3, y: 0 }, 3),
                    (Point { x: 1, y: 1 }, 2),
                    (Point { x: 2, y: 1 }, 4),
                    (Point { x: 3, y: 1 }, 6),
                    (Point { x: 1, y: 2 }, 3),
                    (Point { x: 2, y: 2 }, 6),
                    (Point { x: 3, y: 2 }, 8),
                ]),
            ),
        ];
        for (corners, expected_prefix) in test_cases {
            let prefix = compute_prefix(&corners);
            assert_eq!(prefix, expected_prefix);
        }
    }

    #[test]
    fn test_rectangle() {
        let test_cases = [(
            vec![&Point { x: 3, y: 2 }, &Point { x: 6, y: 4 }],
            Rectangle {
                opposing_corners: vec![&Point { x: 3, y: 2 }, &Point { x: 6, y: 4 }],
                size: 12,
                l: 3,
                t: 2,
                r: 6,
                b: 4,
            },
        )];
        for (points, expected_rectangle) in test_cases {
            let r = Rectangle::new(points);
            assert_eq!(r.size, expected_rectangle.size);
            assert_eq!(r.l, expected_rectangle.l);
            assert_eq!(r.t, expected_rectangle.t);
            assert_eq!(r.r, expected_rectangle.r);
            assert_eq!(r.b, expected_rectangle.b);
        }
    }

    #[test]
    fn test_area_contains() {
        let test_cases = [(
            vec![
                Point { x: 1, y: 0 },
                Point { x: 3, y: 0 },
                Point { x: 3, y: 1 },
                Point { x: 1, y: 1 },
            ],
            vec![
                ((&Point { x: 0, y: 0 }, &Point { x: 1, y: 1 }), false),
                ((&Point { x: 1, y: 0 }, &Point { x: 2, y: 1 }), true),
            ],
        )];

        for (area_points, tests) in test_cases {
            let a = Area::new(&area_points);
            for (r, expected_contains) in tests {
                let rect = Rectangle::new_from_tuple(r);
                let contains = a.contains(rect);
                assert_eq!(contains, expected_contains);
            }
        }
    }
}
