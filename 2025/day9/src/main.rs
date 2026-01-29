use std::{collections::HashMap, fs, time::SystemTime};

#[derive(Debug, PartialEq, Hash, Eq, Clone, Copy)]
struct Point {
    x: u32,
    y: u32,
}

#[derive(Debug)]
struct Area<'a> {
    points: &'a [Point],
    prefix: Vec<Vec<u32>>,
}

impl<'a> Area<'a> {
    // use prefix
    fn contains(&self, rect: (&Point, &Point)) -> bool {
        false
    }

    fn new(points: &'a [Point]) -> Area {
        Area {
            points,
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
    let biggest_green = rectangles.iter().find(|r| area.contains(r.0)).unwrap();

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

fn compute_prefix(corners: &[Point]) -> Vec<Vec<u32>> {
    let prefix = Vec::new();
    let green_or_red_cells: Vec<Point> = get_painted_cells(corners);
    for p in &green_or_red_cells {}

    prefix
}

fn get_painted_cells(corners: &[Point]) -> Vec<Point> {
    let edges = get_edges(corners);
    get_filled_region(edges)
}

fn get_filled_region(edges: Vec<Point>) -> Vec<Point> {
    todo!()
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
    fn test_get_edges() {
        let test_cases = [(
            &[
                Point { x: 7, y: 1 },
                Point { x: 11, y: 1 },
                Point { x: 11, y: 7 },
                Point { x: 7, y: 7 },
            ],
            20,
        )];

        for (corners, expected_num_of_cells) in test_cases {
            let res = get_edges(corners);
            assert_eq!(res.len(), expected_num_of_cells);
        }
    }
}
