use std::{env, fs, time::SystemTime};

static PART2: bool = true;

#[derive(Debug, Clone, PartialEq, Hash, Eq)]
struct Coor {
    x: u32,
    y: u32,
    z: u32,
}

impl Coor {
    fn new(s: String) -> Coor {
        let mut i = s.split(',');
        Coor {
            x: i.next().unwrap().parse::<u32>().unwrap(),
            y: i.next().unwrap().parse::<u32>().unwrap(),
            z: i.next().unwrap().parse::<u32>().unwrap(),
        }
    }
    fn get_distance(&self, c2: &Coor) -> f64 {
        let x_dist = (self.x.abs_diff(c2.x) as u64).pow(2);
        let y_dist = (self.y.abs_diff(c2.y) as u64).pow(2);
        let z_dist = (self.z.abs_diff(c2.z) as u64).pow(2);
        ((x_dist + y_dist + z_dist) as f64).sqrt()
    }
}

fn main() {
    let now = SystemTime::now();
    let args: Vec<String> = env::args().collect();
    let coors: Vec<Coor> = fs::read_to_string(&args[1])
        .expect("Couldnt read inputfile")
        .lines()
        .map(|l| l.to_owned())
        .map(|s| Coor::new(s))
        .collect();

    let runs_by_input = if args[1] == "testinput.txt" {
        10
    } else if args[1] == "input.txt" {
        1000
    } else {
        panic!("Unknown input! ")
    };

    let mut circuits: Vec<Vec<Coor>> = coors.iter().map(|c| vec![c.clone()]).collect();
    let mut sorted_distances = build_distance_map_using_refs(&coors);

    let mut last_jb1 = &Coor { x: 0, y: 0, z: 0 };
    let mut last_jb2 = &Coor { x: 0, y: 0, z: 0 };

    if !PART2 {
        for _ in 0..runs_by_input {
            let ((jb1, jb2), _) = sorted_distances
                .pop()
                .expect("No more connections to be made!");
            _ = make_next_connection(jb1, jb2, &mut circuits)
        }
    } else {
        while circuits.len() > 1 {
            //for ((jb1, jb2), _) in sorted_distances {

            let ((jb1, jb2), _) = sorted_distances
                .pop()
                .expect("No more connections to be made!");
            _ = make_next_connection(jb1, jb2, &mut circuits);
            last_jb1 = jb1;
            last_jb2 = jb2;
        }
    }

    println!("time elapsed: {:?}", now.elapsed().unwrap());
    if !PART2 {
        circuits.sort_by(|a, b| a.len().cmp(&b.len()));
        let longest = &circuits[circuits.len() - 1].len();
        let second_longest = &circuits[circuits.len() - 2].len();
        let third_longest = &circuits[circuits.len() - 3].len();
        println!(
            "Lengths of longest circuits: {}, {}, {}. Product = {}",
            third_longest,
            second_longest,
            longest,
            longest * second_longest * third_longest
        );
    } else {
        println!(
            "last jb1: {:?}, last jb2: {:?}, product of x's = {}",
            last_jb1,
            last_jb2,
            last_jb1.x as u64 * last_jb2.x as u64
        );
    }
}

fn make_next_connection(jb1: &Coor, jb2: &Coor, circuits: &mut Vec<Vec<Coor>>) -> bool {
    let index_of_circuit_containing_jb2 = circuits
        .iter()
        .position(|v| v.contains(jb2))
        .expect(format!("Couldnt find circuit containing v2 {:?}", jb2).as_str());

    let index_of_circuit_containing_jb1 = circuits
        .iter()
        .position(|v| v.contains(jb1))
        .expect(format!("Couldnt find circuit containing v1 {:?}", jb1).as_str());

    if index_of_circuit_containing_jb1 == index_of_circuit_containing_jb2 {
        return false;
    }

    let vals = circuits[index_of_circuit_containing_jb2].clone();

    for coor in vals {
        circuits[index_of_circuit_containing_jb1].push(coor);
    }

    circuits.remove(index_of_circuit_containing_jb2);

    true
}

fn build_distance_map_using_refs(coors: &[Coor]) -> Vec<((&Coor, &Coor), f64)> {
    let mut distance_map = std::collections::HashMap::new();
    println!("in building distance map");
    for c1 in coors {
        for c2 in coors {
            if c1 != c2 {
                if distance_map.contains_key(&(c2, c1)) {
                    continue;
                }
                distance_map.insert((c1, c2), c1.get_distance(&c2));
            }
        }
    }

    let mut distances = Vec::new();
    for (k, v) in distance_map {
        distances.push((k, v));
    }

    distances.sort_by(|a, b| a.1.partial_cmp(&b.1).unwrap());
    distances.reverse();
    distances
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_get_distance() {
        let test_cases = [(
            Coor {
                x: 162,
                y: 817,
                z: 812,
            },
            Coor {
                x: 57,
                y: 618,
                z: 57,
            },
            787.81406435782802671060154068308_f64,
        )];

        for (c1, c2, er) in test_cases {
            let rr = c1.get_distance(&c2);
            assert_eq!(
                rr, er,
                "Expected {er}, got {rr}. c1: {:#?}, c2: {:#?}",
                c1, c2
            );
        }
    }
}
