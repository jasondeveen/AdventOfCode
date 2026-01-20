use std::{collections::HashMap, fs, time::SystemTime};

#[derive(Debug, PartialEq, Hash, Eq)]
struct Point {
    x: u32,
    y: u32,
}

#[derive(Debug)]
struct Area<'a> {
    points: &'a [Point],
    prefix: HashMap<(u32, u32), u64>,
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

// chatGPT: prefix sum/ summed-area table
fn compute_prefix(points: &[Point]) -> HashMap<(u32, u32), u64> {}
