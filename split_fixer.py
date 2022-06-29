path = r"C:\\Users\\Fabio\Downloads\\neon_white_splits\\dirty_rightsplit.lss"
asl_path = r"C:\\DEV\\neonwhite\\neonwhite_master.asl"
asl_path_out = r"C:\\DEV\\neonwhite\\neonwhite_generated.asl"
new_order_path = r"C:\\DEV\\neonwhite\\new_order.txt"
splitinfo_path = r"C:\\DEV\\neonwhite\\splitinfo.txt"
new_splitinfo_path = r"C:\\DEV\\neonwhite\\splitinfo_new.txt"

with open(splitinfo_path, "r") as f:
    splitinfo = f.read()

with open(new_order_path) as f:
    new_order = f.read()

with open(path) as f:
    file_original = f.read()

splitinfo_lookup = {}
for line in splitinfo.splitlines():
    name,_,pb,best = line.split("\t")
    if ':' in pb:
        pb = int(pb.split(':')[0]) * 60 + float(pb.split(':')[1])
    else:
        pb = float(pb)
    splitinfo_lookup[name] = (pb,best)

new_splitifo = []
splitime = 0
for map in new_order.splitlines():
    pb,best = splitinfo_lookup[map]
    splitime += pb

    if pb >= 60:
        pb = str(int(pb/60)) + ":" + str(pb%60)
    else:
        pb = str(pb)
    
    splitime_str = ""
    if splitime >= 60:
        splitime_str = str(int(splitime/60)) + ":" + str(splitime%60)
    else:
        splitime_str = str(splitime)

    new_splitifo.append("\t".join([map,splitime_str,pb,best,'\n']))


with open(new_splitinfo_path,"w") as f:
    new_splitifo
    f.writelines(new_splitifo)
    splitime



with open(asl_path, "r") as f:
    asl_file = f.read()

# TODO: need mapping from ingame names to inengine names
inengine_name = "GRID_BOOP"

asl_file = asl_file.replace("STAGE_NAME", inengine_name)

with open(asl_path_out, "w") as f:
    f.write(asl_file)