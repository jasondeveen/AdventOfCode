use std::{collections::HashMap, fs, time::SystemTime};

#[derive(Debug, PartialEq, Hash, Eq)]
struct Point {
    x: u32,
    y: u32,
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
        "Biggest rectangle: size {} between points {:?} and {:?}",
        biggest.1, biggest.0.0, biggest.0.1
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

    let mut vec = Vec::new();
    for (k, v) in rectangles {
        vec.push((k, v));
    }
    vec.sort_by(|r1, r2| r1.1.partial_cmp(&r2.1).unwrap());
    vec.reverse();

    vec
}
